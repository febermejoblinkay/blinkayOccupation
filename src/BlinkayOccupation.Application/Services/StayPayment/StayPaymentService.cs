using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Occupation;
using BlinkayOccupation.Domain.Repositories.Stay;
using BlinkayOccupation.Domain.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace BlinkayOccupation.Application.Services.StayPayment
{
    public class StayPaymentService : IStayPaymentService
    {
        private readonly IStaysRepository _staysRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOccupationRepository _occupationRepository;
        private readonly ICapacitiesRepository _capacitiesRepository;

        private readonly ILogger<StayPaymentService> _logger;

        public StayPaymentService(
            IStaysRepository staysRepository,
            IOccupationRepository occupationRepository,
            IUnitOfWork unitOfWork,
            ICapacitiesRepository capacitiesRepository
            )
        {
            _staysRepository = staysRepository;
            _occupationRepository = occupationRepository;
            _unitOfWork = unitOfWork;
            _capacitiesRepository = capacitiesRepository;
        }

        public async Task ProcessInitEndPaymentStay()
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var staysToProcess = await _staysRepository.GetStaysToProcessPaymentsAsync(_unitOfWork.Context);

                foreach (var stay in staysToProcess)
                {
                    var installationDateNow = stay.Installation.DateTimeNow();
                    var stayPkRights = stay.StaysParkingRights.Select(x => x.ParkingRight).ToList();
                    var currentParkingRight = stayPkRights.FirstOrDefault(x => x.ValidTo.Value > installationDateNow);
                    var currentTariff = currentParkingRight?.Tariff;
                    var occupationToUpdate = await _occupationRepository.GetOccupationsAvailable(installationDateNow.Date, stay.InstallationId, stay.ZoneId, currentTariff.Id, _unitOfWork.Context);
                    var currentOccupation = occupationToUpdate.FirstOrDefault();

                    if (currentTariff.PaymentApplyAllDay == true)
                    {
                        var existingOccupations = await _occupationRepository.GetExistingOccupationsByDate(
                            currentParkingRight.ValidFrom,
                            currentParkingRight.ValidTo.Value,
                            stay.InstallationId,
                            stay.ZoneId,
                            currentTariff.Id,
                            _unitOfWork.Context
                        );

                        var allDates = Enumerable.Range(0, (currentParkingRight.ValidTo.Value - currentParkingRight.ValidFrom).Days + 1)
                            .Select(offset => currentParkingRight.ValidFrom.AddDays(offset).Date)
                            .ToList();

                        var existingDates = existingOccupations.Select(o => o.Date.Value.Date).ToHashSet();
                        var newOccupations = new List<Occupations>();
                        var capacity = await _capacitiesRepository.GetAvailableCapacities(stay.InstallationId, stay.ZoneId, currentTariff.Id, stay.EntryDate, stay.ExitDate, _unitOfWork.Context);

                        foreach (var date in allDates)
                        {
                            var occupation = existingOccupations.FirstOrDefault(o => o.Date.Value.Date == date);

                            if (occupation != null)
                            {
                                occupation.PaidOccupation += 1;
                            }
                            else
                            {
                                newOccupations.Add(new Occupations
                                {
                                    Id = Guid.CreateVersion7().ToString(),
                                    Date = date,
                                    InstallationId = stay.InstallationId,
                                    ZoneId = stay.ZoneId,
                                    TariffId = currentTariff.Id,
                                    PaidOccupation = 1,
                                    PaidRealOccupation = 0,
                                    UnpaidRealOccupation = 0,
                                    Total = capacity != null ? capacity.Count : 0
                                });
                            }
                        }

                        if (existingOccupations.Count > 0)
                        {
                            await _occupationRepository.UpdateRangeAsync(existingOccupations, _unitOfWork.Context);
                        }

                        if (newOccupations.Count > 0)
                        {
                            await _occupationRepository.AddRangeAsync(newOccupations, _unitOfWork.Context);
                        }

                        stay.InitPaymentProcessed = true;
                        await _staysRepository.UpdateAsync(stay, _unitOfWork.Context);
                        await _unitOfWork.Context.SaveChangesAsync();
                    }

                    if (stay.InitPaymentProcessed == false && currentTariff != null && currentTariff.PaymentApplyAllDay == false && installationDateNow > stay.InitPaymentDate)
                    {
                        currentOccupation.PaidOccupation += 1;
                        stay.InitPaymentProcessed = true;
                        if (!string.IsNullOrWhiteSpace(stay.EntryEventId) && string.IsNullOrWhiteSpace(stay.ExitEventId))
                        {
                            currentOccupation.PaidRealOccupation += 1;
                            currentOccupation.UnpaidRealOccupation --;
                        }

                        await _occupationRepository.UpdateRangeAsync(new List<Domain.Models.Occupations> { currentOccupation }, _unitOfWork.Context);
                        await _staysRepository.UpdateAsync(stay, _unitOfWork.Context);
                        await _unitOfWork.Context.SaveChangesAsync();
                    }

                    if (stay.InitPaymentProcessed == true && installationDateNow > stay.EndPaymentDate)
                    {
                        currentOccupation.PaidOccupation --;
                        stay.EndPaymentProcessed = true;

                        if (!string.IsNullOrWhiteSpace(stay.EntryEventId))
                        {
                            currentOccupation.PaidRealOccupation --;
                            currentOccupation.UnpaidRealOccupation += 1;
                        }

                        await _occupationRepository.UpdateRangeAsync(new List<Domain.Models.Occupations> { currentOccupation }, _unitOfWork.Context);
                        await _staysRepository.UpdateAsync(stay, _unitOfWork.Context);
                        await _unitOfWork.Context.SaveChangesAsync();
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "PaymentProcessWorker-ProcessInitEndPaymentStay: An error occured when trying to process stays payments, transaction rollbacked.");
            }
        }
    }
}
