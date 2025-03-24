using BlinkayOccupation.Application.Exceptions;
using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Strategies;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Capacity;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.ParkingEvent;
using BlinkayOccupation.Domain.Repositories.ParkingRight;
using BlinkayOccupation.Domain.Repositories.Stay;
using BlinkayOccupation.Domain.Repositories.StayParkingRight;
using BlinkayOccupation.Domain.Repositories.Zone;
using BlinkayOccupation.Domain.UnitOfWork;

namespace BlinkayOccupation.Application.Services.Stay
{
    public class StayService : IStayService
    {
        private readonly IParkingEventsRepository _parkingEventsRepository;
        private readonly IParkingRightsRepository _parkingRightsRepository;
        private readonly IStaysRepository _staysRepository;
        private readonly IInstallationRepository _installationRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly IStayParkingRightsRepository _stayPkRightRepository;
        private readonly ICapacitiesRepository _capacitiesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOccupationStrategyFactory _occupationStrategyFactory;

        public StayService(
            IParkingEventsRepository parkingEventsRepository,
            IParkingRightsRepository parkingRightsRepository,
            IStaysRepository staysRepository,
            IInstallationRepository installationRepository,
            IZoneRepository zoneRepository,
            IStayParkingRightsRepository stayPRightRepository,
            ICapacitiesRepository capacitiesRepository,
            IUnitOfWork unitOfWork,
            IOccupationStrategyFactory occupationStrategyFactory)
        {
            _parkingEventsRepository = parkingEventsRepository;
            _parkingRightsRepository = parkingRightsRepository;
            _staysRepository = staysRepository;
            _installationRepository = installationRepository;
            _zoneRepository = zoneRepository;
            _stayPkRightRepository = stayPRightRepository;
            _capacitiesRepository = capacitiesRepository;
            _unitOfWork = unitOfWork;
            _occupationStrategyFactory = occupationStrategyFactory;
        }

        public async Task<string> AddStay(AddStayRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingPkEvents = await GetExistingParkingEventsAsync(request.EntryEventId, request.ExitEventId);
                var existingPkRights = await GetExistingParkingRightsAsync(request.ParkingRightIds);

                var installation = await GetInstallationAsync(request.InstallationId);
                var zone = await GetZoneAsync(request.ZoneId);

                await CheckIfPkRightsAlreadyExistInOtherStay(request.ParkingRightIds);

                var stay = CreateStay(request, existingPkEvents, installation, zone);

                await _staysRepository.AddAsync(stay, _unitOfWork.Context);

                await HandleStayParkingRightsAsync(stay, existingPkRights);

                var oldHasPayment = existingPkRights?.Count > 0;
                var oldStateObj = StayState.FromStay(stay, oldHasPayment);
                var oldState = oldStateObj.ToString();

                //los inserts siempre van a ser: new stay => N
                var newState = "N";
                var strategy = _occupationStrategyFactory.GetStrategy(oldState, newState);
                var entryEvent = existingPkEvents.FirstOrDefault(x => x?.Id == stay.EntryEventId);
                var exitEvent = existingPkEvents.FirstOrDefault(x => x?.Id == stay.ExitEventId);
                DateTime? paymentEndDate = null;
                //nos fijamos si la fecha actual es menor al validTo
                //if (request.ExitDate.HasValue)
                //{
                paymentEndDate = existingPkRights?.FirstOrDefault(x => DateTime.UtcNow < x.ValidTo)?.ValidTo;
                //}

                var tariffId = GetTariffId(existingPkRights, stay, oldStateObj);

                await strategy.ExecuteAsync(stay, tariffId, oldState, newState, _unitOfWork.Context, paymentEndDate);

                await _unitOfWork.CommitTransactionAsync();

                return stay.Id;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<string> UpdateStay(UpdateStayRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var stay = await _staysRepository.GetByIdAsync(request.StayId, _unitOfWork.Context);
                if (stay == null) throw new StayNotFoundException("Stay not found.");

                bool oldHasPayment = await CheckIfStayHadPayment(request, stay.StaysParkingRights);
                var oldStateObj = StayState.FromStay(stay, oldHasPayment);
                string oldState = oldStateObj.ToString();

                await CheckIfPkRightsAlreadyExistInOtherStay(request.ParkingRightIds);

                var existingPkEvents = await GetExistingParkingEventsAsync(request.EntryEventId, request.ExitEventId);
                var existingPkRights = await GetExistingParkingRightsAsync(request.ParkingRightIds);

                var oldPkRights = stay.StaysParkingRights.Select(x => x.ParkingRight).ToList();
                if (oldPkRights?.Count > 0)
                {
                    existingPkRights.AddRange(oldPkRights);
                }

                UpdateStayEntity(stay, request, existingPkEvents);

                await _staysRepository.UpdateAsync(stay, _unitOfWork.Context);

                await HandleStayParkingRightsAsync(stay, existingPkRights);

                var newHasPayment = await CheckIfStayHasPayment(stay, request, existingPkRights);
                //newHasPayment = si ya el stay tenia pago o si llega un nuevo pago que cubre el periodo
                var newStateObj = StayState.FromStay(stay, newHasPayment);
                var newState = newStateObj.ToString();
                var strategy = _occupationStrategyFactory.GetStrategy(oldState, newState);
                var entryEvent = existingPkEvents.FirstOrDefault(x => x?.Id == stay?.EntryEventId);
                var exitEvent = existingPkEvents.FirstOrDefault(x => x?.Id == stay.ExitEventId);

                DateTime? paymentEndDate = null;
                if (request.ExitDate.HasValue || existingPkRights?.Count > 0)
                {
                    if (request.ExitDate.HasValue || stay.ExitDate.HasValue)
                    {
                        var exitDate = request.ExitDate ?? stay.ExitDate;
                        paymentEndDate = existingPkRights.FirstOrDefault(x => exitDate.Value <= x.ValidTo)?.ValidTo;
                    }
                    else if (request.EntryDate.HasValue || stay.EntryDate.HasValue)
                    {
                        var entryDate = request.EntryDate ?? stay.EntryDate;
                        paymentEndDate = existingPkRights.FirstOrDefault(x => entryDate.Value >= x.ValidFrom && entryDate.Value <= x.ValidTo)?.ValidTo;
                    }
                    else
                    {
                        throw new ParkingRightsNoValidEndDateException("The Parking right has no validTo date defined.");
                    }
                }

                var tariffId = GetTariffId(existingPkRights, stay, oldStateObj.HasPayment ? oldStateObj : newStateObj);

                await strategy.ExecuteAsync(stay, tariffId, oldState, newState, _unitOfWork.Context, paymentEndDate);

                await _unitOfWork.CommitTransactionAsync();

                return stay.Id;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error updating Stay: " + ex.Message, ex);
            }
        }

        private string? GetTariffId(List<ParkingRights>? existingPkRights, Stays stay, StayState stateObj)
        {
            if (stateObj.HasPayment && stateObj.HasEntry && stateObj.HasExit)
            {
                return existingPkRights?.FirstOrDefault(x => stay.EntryDate.Value >= x.ValidFrom && stay.ExitDate.Value <= x.ValidTo.Value)?.TariffId;
            }
            else if (stateObj.HasPayment && stateObj.HasEntry && !stateObj.HasExit)
            {
                return existingPkRights?.FirstOrDefault(x => x.ValidTo.HasValue && stay.EntryDate.Value < x.ValidTo.Value)?.TariffId;
            }
            else if (stateObj.HasPayment && !stateObj.HasEntry && stateObj.HasExit)
            {
                return existingPkRights?.FirstOrDefault(x => x.ValidTo.HasValue && stay.ExitDate.Value < x.ValidTo.Value)?.TariffId;
            }
            else
            {
                return existingPkRights?.FirstOrDefault(x => DateTime.UtcNow < x.ValidTo)?.TariffId;
            }
        }

        private async Task<bool> CheckIfStayHasPayment(Stays stay, UpdateStayRequest request, List<ParkingRights> currentPkRights)
        {
            if (request.EntryDate.HasValue || stay.EntryDate.HasValue)
            {
                DateTime? entryDate = request.EntryDate ?? stay.EntryDate;
                DateTime? exitDate = request.ExitDate ?? stay.ExitDate;

                if (entryDate.HasValue && exitDate.HasValue)
                {
                    return currentPkRights.Any(x => entryDate.Value >= x?.ValidFrom && exitDate.Value <= x?.ValidTo);
                }
                if (entryDate.HasValue)
                {
                    return currentPkRights.Any(x => entryDate.Value >= x?.ValidFrom);
                }
                if (exitDate.HasValue)
                {
                    return currentPkRights.Any(x => exitDate.Value <= x?.ValidTo);
                }
            }

            return false;
        }

        private async Task<bool> CheckIfStayHadPayment(UpdateStayRequest request, ICollection<StaysParkingRights> stayParkingRights = null)
        {
            if (stayParkingRights?.Count > 0 && request.EntryDate.HasValue && request.ExitDate.HasValue)
            {
                return stayParkingRights.Any(x => request.EntryDate.Value >= x?.ParkingRight?.ValidFrom && request.ExitDate.Value <= x?.ParkingRight?.ValidTo);
            }
            else if (stayParkingRights?.Count > 0 && request.EntryDate.HasValue && !request.ExitDate.HasValue)
            {
                return stayParkingRights.Any(x => request.EntryDate.Value >= x?.ParkingRight?.ValidFrom);
            }
            else if (stayParkingRights?.Count > 0 && !request.EntryDate.HasValue && request.ExitDate.HasValue)
            {
                return stayParkingRights.Any(x => request.ExitDate.Value <= x?.ParkingRight?.ValidTo);
            }
            else
            {
                return false;
            }
        }

        private async Task CheckIfPkRightsAlreadyExistInOtherStay(List<string> pkRightIds)
        {
            if (pkRightIds?.Count > 0)
            {
                var existingPkRightsInStay = await _stayPkRightRepository.CheckParkingRightExistsInOtherStayAsync(pkRightIds, _unitOfWork.Context);
                if (existingPkRightsInStay?.Count > 0)
                {
                    throw new ParkingRightsExistsInStayException();
                }
            }
        }

        private async Task<List<ParkingEvents?>> GetExistingParkingEventsAsync(string entryEventId, string exitEventId)
        {
            var eventIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(entryEventId)) eventIds.Add(entryEventId);
            if (!string.IsNullOrWhiteSpace(exitEventId)) eventIds.Add(exitEventId);

            if (eventIds?.Count > 0)
            {
                var events = await _parkingEventsRepository.GetByIdsAsync(eventIds, _unitOfWork.Context);
                if (events?.Count == 0)
                {
                    throw new ParkingEventsNotFoundException("No parking events found.");
                }
                return events;
            }

            return new List<ParkingEvents?>();
        }

        private async Task<List<ParkingRights>> GetExistingParkingRightsAsync(List<string> parkingRightIds)
        {
            if (parkingRightIds.All(x => !string.IsNullOrWhiteSpace(x)) && parkingRightIds?.Count > 0)
            {
                var rights = await _parkingRightsRepository.GetByIdsAsync(parkingRightIds, _unitOfWork.Context);
                if (rights?.Count == 0)
                {
                    throw new ParkingRightsNotFoundException("No parking rights found.");
                }
                return rights;
            }

            return new List<ParkingRights>();
        }

        private async Task<Installations> GetInstallationAsync(string installationId)
        {
            var installation = await _installationRepository.GetByIdAsync(installationId, _unitOfWork.Context);
            if (installation == null)
            {
                throw new InstallationNotFoundException("Installation not found.");
            }
            return installation;
        }

        private async Task<Zones> GetZoneAsync(string zoneId)
        {
            var zone = await _zoneRepository.GetByIdAsync(zoneId, _unitOfWork.Context);
            if (zone == null)
            {
                throw new ZoneNotFoundException("Zone not found.");
            }
            return zone;
        }

        private Stays CreateStay(AddStayRequest request, List<ParkingEvents?> existingPkEvents, Installations installation, Zones zone)
        {
            return new Stays
            {
                Id = Guid.CreateVersion7().ToString(),
                CaseId = request.CaseId.HasValue && (CameraOperationCase)request.CaseId.Value != CameraOperationCase.Undefined ? request.CaseId.Value : (int?)null,
                InstallationId = request.InstallationId,
                ZoneId = request.ZoneId,
                EntryEventId = !string.IsNullOrWhiteSpace(request.EntryEventId) && existingPkEvents.Any(x => x.Id == request.EntryEventId) ? request.EntryEventId : null,
                ExitEventId = !string.IsNullOrWhiteSpace(request.ExitEventId) && existingPkEvents.Any(x => x.Id == request.ExitEventId) ? request.ExitEventId : null,
                EntryDate = request.EntryDate,
                ExitDate = request.ExitDate
            };
        }

        private void UpdateStayEntity(Stays stay, UpdateStayRequest request, List<ParkingEvents?> existingPkEvents)
        {
            stay.EntryEventId = !string.IsNullOrWhiteSpace(request.EntryEventId) && existingPkEvents.Any(x => x.Id == request.EntryEventId) ? request.EntryEventId : stay.EntryEventId;
            stay.ExitEventId = !string.IsNullOrWhiteSpace(request.ExitEventId) && existingPkEvents.Any(x => x.Id == request.ExitEventId) ? request.ExitEventId : stay.ExitEventId;
            stay.EntryDate = request.EntryDate ?? stay.EntryDate;
            stay.ExitDate = request.ExitDate ?? stay.ExitDate;
            stay.CaseId = request.CaseId.HasValue && (CameraOperationCase)request.CaseId.Value != CameraOperationCase.Undefined ? request.CaseId.Value : stay.CaseId;
        }

        private async Task HandleStayParkingRightsAsync(Stays stay, List<ParkingRights> existingPkRights)
        {
            var existingPRightIds = stay.StaysParkingRights.Select(x => x.ParkingRightId).ToList() ?? new List<string?>();
            if (existingPkRights?.Count > 0)
            {
                var newRelations = existingPkRights.Where(x => !existingPRightIds.Contains(x.Id)).Select(item => new StaysParkingRights
                {
                    Id = Guid.CreateVersion7().ToString(),
                    StayId = stay.Id,
                    ParkingRightId = item.Id
                }).ToList();

                if (newRelations?.Count > 0)
                {
                    await _stayPkRightRepository.AddRangeAsync(newRelations, _unitOfWork.Context);
                }
            }
        }
    }
}
