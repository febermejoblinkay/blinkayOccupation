using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.Occupation;
using BlinkayOccupation.Domain.Repositories.OccupationSnapshot;
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
        private readonly IOccupationSnapshotRepository _occupationSnapshotRepository;
        private readonly IInstallationRepository _installationsRepository;

        private readonly ILogger<StayPaymentService> _logger;

        public StayPaymentService(
            IStaysRepository staysRepository,
            IOccupationRepository occupationRepository,
            IUnitOfWork unitOfWork,
            ICapacitiesRepository capacitiesRepository,
            IOccupationSnapshotRepository occupationSnapshotRepository,
            ILogger<StayPaymentService> logger,
            IInstallationRepository installationsRepository
            )
        {
            _staysRepository = staysRepository;
            _occupationRepository = occupationRepository;
            _unitOfWork = unitOfWork;
            _capacitiesRepository = capacitiesRepository;
            _occupationSnapshotRepository = occupationSnapshotRepository;
            _logger = logger;
            _installationsRepository = installationsRepository;
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
                        var existingOccupationsHasChanged = false;
                        foreach (var date in allDates)
                        {
                            var occupation = existingOccupations.FirstOrDefault(o => o.Date.Value.Date == date);

                            if (occupation != null && installationDateNow.Date > occupation.Date.Value.Date)
                            {
                                occupation.PaidOccupation += 1;
                                existingOccupationsHasChanged = true;
                            }

                            if (occupation == null)
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

                        if (existingOccupations.Count > 0 && existingOccupationsHasChanged)
                        {
                            await _occupationRepository.UpdateRangeAsync(existingOccupations, _unitOfWork.Context);
                        }

                        if (newOccupations.Count > 0)
                        {
                            await _occupationRepository.AddRangeAsync(newOccupations, _unitOfWork.Context);
                        }

                        if ((existingOccupations.Count > 0 && existingOccupationsHasChanged) || newOccupations.Count > 0)
                        {
                            stay.InitPaymentProcessed = true;
                            await _staysRepository.UpdateAsync(stay, _unitOfWork.Context);
                            await _unitOfWork.Context.SaveChangesAsync();
                        }
                    }

                    if (stay.InitPaymentProcessed == false && currentTariff != null && currentTariff.PaymentApplyAllDay == false && installationDateNow > stay.InitPaymentDate)
                    {
                        currentOccupation.PaidOccupation += 1;
                        stay.InitPaymentProcessed = true;
                        if (!string.IsNullOrWhiteSpace(stay.EntryEventId) && string.IsNullOrWhiteSpace(stay.ExitEventId))
                        {
                            currentOccupation.PaidRealOccupation += 1;
                            currentOccupation.UnpaidRealOccupation--;
                        }

                        await _occupationRepository.UpdateRangeAsync(new List<Domain.Models.Occupations> { currentOccupation }, _unitOfWork.Context);
                        await _staysRepository.UpdateAsync(stay, _unitOfWork.Context);
                        await _unitOfWork.Context.SaveChangesAsync();
                    }

                    if (stay.InitPaymentProcessed == true && installationDateNow > stay.EndPaymentDate)
                    {
                        currentOccupation.PaidOccupation--;
                        stay.EndPaymentProcessed = true;

                        if (!string.IsNullOrWhiteSpace(stay.EntryEventId))
                        {
                            currentOccupation.PaidRealOccupation--;
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

        public async Task ProcessOccupationsSnapshot()
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var currentDateTime = DateTime.UtcNow;
                var occupations = await _occupationRepository.GetByDate(currentDateTime, _unitOfWork.Context);

                var snapshots = occupations.Select(occupation => new OccupationsSnapshots
                {
                    Id = Guid.CreateVersion7().ToString(),
                    OccupationDate = occupation.Date,
                    SnapshotDate = currentDateTime,
                    InstallationId = occupation.InstallationId,
                    ZoneId = occupation.ZoneId,
                    TariffId = occupation.TariffId,
                    PaidRealOccupation = occupation.PaidRealOccupation,
                    UnpaidRealOccupation = occupation.UnpaidRealOccupation,
                    PaidOccupation = occupation.PaidOccupation,
                    Total = occupation.Total
                }).ToList();

                if (snapshots.Any())
                {
                    await _occupationSnapshotRepository.AddRangeAsync(snapshots, _unitOfWork.Context);
                    await _unitOfWork.Context.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("ProcessOccupationsSnapshot: A new snapshot of occupations has been created with {count} records.", snapshots.Count);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "ProcessOccupationsSnapshot: An error occured when processing occupations snapshot. Transaction rollbacked.");
            }
        }

        public async Task CloneOccupationForInstallation(Installations installation)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var now = installation.DateTimeNow();
                var yesterday = now.AddDays(-1).Date;
                var today = now.Date;

                var yesterdayOccupations = await _occupationRepository.GetOccupationsByDateAndInsId(yesterday, installation.Id, _unitOfWork.Context);

                if (!yesterdayOccupations.Any())
                {
                    _logger.LogWarning("PaymentProcessWorker-CloneOccupationForInstallation: There are no occupations for date: {yesterday} and Installation: {installation}", yesterday, installation.Name);
                    return;
                }

                var occupationsToUpdate = new List<Occupations>();
                var occupationsToCreate = new List<Occupations>();

                var listToFilter = yesterdayOccupations
                                      .Select(x => new Tuple<DateTime?, string, string, string?>(
                                                                                   x.Date,
                                                                                   x.InstallationId,
                                                                                   x.ZoneId,
                                                                                   x.TariffId))
                                      .Distinct()
                                      .ToList();

                var todayOccupations = await _occupationRepository.GetOccupationsByFiltersAsync(listToFilter, _unitOfWork.Context);

                foreach (var yesterdayOccupation in yesterdayOccupations)
                {
                    var existingOccupation = todayOccupations
                                                    .FirstOrDefault(s =>
                                                        s.Date == yesterdayOccupation.Date &&
                                                        s.InstallationId == yesterdayOccupation.InstallationId &&
                                                        s.ZoneId == yesterdayOccupation.ZoneId &&
                                                        (s.TariffId == yesterdayOccupation.TariffId || s.TariffId == null));

                    if (existingOccupation != null)
                    {
                        existingOccupation.PaidRealOccupation = yesterdayOccupation.PaidRealOccupation;
                        existingOccupation.UnpaidRealOccupation = yesterdayOccupation.UnpaidRealOccupation;
                        existingOccupation.Total = yesterdayOccupation.Total;
                        occupationsToUpdate.Add(existingOccupation);
                    }
                    else
                    {
                        occupationsToCreate.Add(new Occupations
                        {
                            Id = Guid.CreateVersion7().ToString(),
                            Date = today,
                            InstallationId = yesterdayOccupation.InstallationId,
                            ZoneId = yesterdayOccupation.ZoneId,
                            TariffId = yesterdayOccupation.TariffId,
                            PaidRealOccupation = yesterdayOccupation.PaidRealOccupation,
                            UnpaidRealOccupation = yesterdayOccupation.UnpaidRealOccupation,
                            PaidOccupation = 0,
                            Total = yesterdayOccupation.Total
                        });
                    }
                }

                if (occupationsToCreate.Any())
                {
                    await _occupationRepository.AddRangeAsync(occupationsToCreate, _unitOfWork.Context);
                }

                if (occupationsToUpdate.Any())
                {
                    await _occupationRepository.UpdateRangeAsync(occupationsToUpdate, _unitOfWork.Context);
                }

                await _unitOfWork.Context.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("PaymentProcessWorker-CloneOccupationForInstallation: Finished clonning occupations in database.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "PaymentProcessWorker-CloneOccupationForInstallation: An error occured when trying to process stays payments, transaction rollbacked.");
            }
        }

        public async Task<List<Installations>> GetAllInstallationsAsync()
        {
            var installations = new List<Installations>();
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                installations = await _installationsRepository.GetAllAsync(_unitOfWork.Context);
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "An error occured when trying to get installations.");
            }

            return installations;
        }
    }
}
