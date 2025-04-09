using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.AzureBlob;
using BlinkayOccupation.Application.Services.Stay;
using BlinkayOccupation.Domain.Helpers;
using BlinkayOccupation.Domain.Models;
using BlinkayOccupation.Domain.Repositories.Attachment;
using BlinkayOccupation.Domain.Repositories.InputDevice;
using BlinkayOccupation.Domain.Repositories.Installation;
using BlinkayOccupation.Domain.Repositories.ParkingEvent;
using BlinkayOccupation.Domain.Repositories.ParkingRight;
using BlinkayOccupation.Domain.Repositories.Space;
using BlinkayOccupation.Domain.Repositories.StreetSection;
using BlinkayOccupation.Domain.Repositories.Tariff;
using BlinkayOccupation.Domain.Repositories.VehicleEvent;
using BlinkayOccupation.Domain.Repositories.Zone;
using BlinkayOccupation.Domain.UnitOfWork;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace BlinkayOccupation.Application.Services.ParkingEvent
{
    public class ParkingEventsService : IParkingEventsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInputDeviceRepository _inputDeviceRepository;
        private readonly IVehicleEventsRepository _vechicleEventsRepository;
        private readonly IParkingEventsRepository _parkingEventsRepository;
        private readonly IParkingRightsRepository _parkingRightsRepository;
        private readonly IInstallationRepository _installationRepository;
        private readonly ITariffRepository _tariffRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly IStreetSectionRepository _streetSectionRepository;
        private readonly ISpaceRepository _spaceRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IBlobStorage _blobStorage;

        public const string NAME = "blinkay.control";
        private static readonly Meter _meter = new(NAME);
        private static readonly Counter<long> _parkingEvents = _meter.CreateCounter<long>(NAME + ".parking_events");

        private readonly ILogger<StayService> _logger;

        public ParkingEventsService(
            IUnitOfWork unitOfWork,
            IInputDeviceRepository inputDeviceRepository,
            IVehicleEventsRepository vehicleEventsRepository,
            IParkingEventsRepository parkingEventsRepository,
            IParkingRightsRepository parkingRightsRepository,
            IInstallationRepository installationRepository,
            ITariffRepository tariffRepository,
            IZoneRepository zoneRepository,
            IStreetSectionRepository streetSectionRepository,
            ISpaceRepository spaceRepository,
            ILogger<StayService> logger,
            IBlobStorage blobStorage,
            IAttachmentRepository attachmentRepository)
        {
            _unitOfWork = unitOfWork;
            _inputDeviceRepository = inputDeviceRepository;
            _vechicleEventsRepository = vehicleEventsRepository;
            _parkingEventsRepository = parkingEventsRepository;
            _parkingRightsRepository = parkingRightsRepository;
            _installationRepository = installationRepository;
            _tariffRepository = tariffRepository;
            _zoneRepository = zoneRepository;
            _streetSectionRepository = streetSectionRepository;
            _spaceRepository = spaceRepository;
            _logger = logger;
            _blobStorage = blobStorage;
            _attachmentRepository = attachmentRepository;
        }

        public async Task<string> CreateParkingEvent(CreateVehicleParkingRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var inputDeviceFromBd = await _inputDeviceRepository.GetByIdAsync(request.Device.Id, _unitOfWork.Context);
                var installation = await _installationRepository.GetByIdAsync(request.InstallationId, _unitOfWork.Context);
                if (inputDeviceFromBd != null)
                {
                    //if (request.Device.Date < inputDeviceFromBd.LastReceivedEvent)
                    //{
                    //    throw new LastReceivedEventDateException($"Last received event ({request.Device.Date:s}) can't be before {inputDeviceFromBd.LastReceivedEvent:s}.");
                    //}
                    //else
                    //{
                    inputDeviceFromBd.LastReceivedEvent = request.Device.Date;
                    await _inputDeviceRepository.UpdateAsync(inputDeviceFromBd, _unitOfWork.Context);
                    //}
                }

                var duplicated = await FindDuplicateByPlate(request);
                if (duplicated is not null)
                {
                    return await HandleDuplicated(duplicated, request);
                }

                var attachments = await StorePictures(inputDeviceFromBd, request, installation);
                var newId = await HandleParkingEvent(request, installation, attachments);

                await _unitOfWork.CommitTransactionAsync();

                return newId;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "BlinkayOccupation-CreateParkingEvent: An error occured when trying to add an stay, transaction rollbacked.");
                throw;
            }
        }

        private async Task<List<Attachments>?> StorePictures(InputDevices device, CreateVehicleParkingRequest request, Installations installation)
        {
            if (request.Pictures == null || !request.Pictures.Any())
            {
                _logger.LogWarning("No attachments received");
                return null;
            }

            var attachments = new List<Attachments>(2);
            foreach (var picture in request.Pictures)
            {
                var id = IBlobStorage.CreateId();
                await _blobStorage.SaveAsync(id, picture);
                var attachment = new Attachments
                {
                    Id = Guid.CreateVersion7().ToString(),
                    Device = device,
                    Created = request.Device.Date,
                    StorageId = id,
                    Direction = request.Direction,
                    Installation = installation
                };

                attachments.Add(attachment);
                await _attachmentRepository.AddAsync(attachment, _unitOfWork.Context);
            }

            return attachments;
        }

        private async Task<string> HandleParkingEvent(CreateVehicleParkingRequest request, Installations installation, List<Attachments>? attachments)
        {
            var openByPlate = await _parkingEventsRepository.ListOpenByPlate(new(installation, request.Device.Date), new[] { request.Vehicle.Plate }, _unitOfWork.Context);
            var tariff = await _tariffRepository.FindByVehicleMake(installation.Id, request.Vehicle.Make, _unitOfWork.Context);

            ParkingEvents parkingEvent;
            IEnumerable<ParkingEvents> toClose;
            Zones zone = null;
            StreetSections streetSection = null;
            Spaces space = null;

            if (!string.IsNullOrWhiteSpace(request.ParkingArea.ZoneId))
            {
                zone = await _zoneRepository.GetByIdAsync(request.ParkingArea.ZoneId, _unitOfWork.Context);
            }

            if (!string.IsNullOrWhiteSpace(request.ParkingArea.StreetSectionId))
            {
                streetSection = await _streetSectionRepository.GetByIdAsync(request.ParkingArea.StreetSectionId, _unitOfWork.Context);
            }

            if (!string.IsNullOrWhiteSpace(request.ParkingArea.SpaceId))
            {
                space = await _spaceRepository.GetByIdAsync(request.ParkingArea.SpaceId, _unitOfWork.Context);
            }

            if (request.Direction == (int)ParkingEventDirection.Enter)
            {
                AddParkingEvent(installation, "vehicle_plate");
                parkingEvent = new ParkingEvents
                {
                    Id = Guid.CreateVersion7().ToString(),
                    Tariff = tariff,
                    InstallationId = installation.Id,
                    Installation = installation,
                    Zone = zone,
                    ZoneId = zone != null ? zone.Id : null,
                    StreetSection = streetSection,
                    StreetSectionId = streetSection != null ? streetSection.Id : null,
                    Space = space,
                    SpaceId = space != null ? space.Id : null,
                    DeviceId = request.Device.Id,
                    Type = (int)ParkingEventType.Space
                };

                AssignEnterExitState(parkingEvent, (ParkingEventDirection)request.Direction, request.Device.Date, ParkingEventClosingReason.Undefined);
                toClose = openByPlate;
            }
            else
            {
                var lastOpen = openByPlate.LastOrDefault();

                if (lastOpen is null)
                {
                    parkingEvent = new ParkingEvents
                    {
                        Id = Guid.CreateVersion7().ToString(),
                        Tariff = tariff,
                        InstallationId = installation.Id,
                        ZoneId = zone != null ? zone.Id : null,
                        StreetSectionId = streetSection != null ? streetSection.Id : null,
                        SpaceId = space != null ? space.Id : null,
                        DeviceId = request.Device.Id,
                        Type = (int)ParkingEventType.Space
                    };

                    AssignEnterExitState(parkingEvent, (ParkingEventDirection)request.Direction, request.Device.Date, ParkingEventClosingReason.Undefined);
                    toClose = [];
                }
                else
                {
                    parkingEvent = lastOpen;
                    parkingEvent.Type = (int)ParkingEventType.Space;
                    AssignEnterExitState(parkingEvent, (ParkingEventDirection)request.Direction, request.Device.Date, ParkingEventClosingReason.Exit);
                    toClose = openByPlate.Take(openByPlate.Count - 1);
                }
            }

            await CloseOpenedParkingEvents(toClose, request.Device.Date);
            //await AssignParkingRight(parkingEvent, request);

            var existingPkEvent = await _parkingEventsRepository.GetByIdAsync(parkingEvent.Id, _unitOfWork.Context);

            if (existingPkEvent is null)
            {
                await _parkingEventsRepository.AddAsync(parkingEvent, _unitOfWork.Context);
            }
            else
            {
                await _parkingEventsRepository.UpdateAsync(parkingEvent, _unitOfWork.Context);
            }
            //await _unitOfWork.Context.SaveChangesAsync();

            return parkingEvent.Id;
        }

        public ParkingEventDirection Direction(ParkingEvents parkingEvent)
        {

            return parkingEvent.Exit is not null
                ? ParkingEventDirection.Exit
                : parkingEvent.Enter is not null
                    ? ParkingEventDirection.Enter
                    : ParkingEventDirection.Undefined;

        }

        private async Task AssignParkingRight(ParkingEvents parkingEvent, CreateVehicleParkingRequest request)
        {
            if (parkingEvent.ParkingRight is not null)
            {
                return;
            }

            parkingEvent.Plate = request.Vehicle.Plate;
            var parkingRight = await _parkingRightsRepository.GetByValidPlateAsync(parkingEvent.Plate!, request.Device.Date, parkingEvent.Installation.ConfigurationParkingRightMatchOffset, _unitOfWork.Context);
            if (parkingRight is null)
            {
                return;
            }

            parkingEvent.ParkingRight = parkingRight;
            parkingEvent.Tariff = parkingRight.Tariff;

            if (string.IsNullOrEmpty(parkingEvent.Plate))
            {
                return;
            }

            parkingEvent.Plate = GetBestPlate(parkingRight.Plates, parkingEvent);
            parkingEvent.PlateConfidence = 1;

            if (parkingRight is not null)
            {
                await _parkingRightsRepository.AddAsync(parkingRight, _unitOfWork.Context);
            }
        }

        private async Task CloseOpenedParkingEvents(IEnumerable<ParkingEvents> toClose, DateTime date)
        {
            foreach (var parkingEvent in toClose)
            {
                await Close(parkingEvent, date, ParkingEventClosingReason.DuplicatedEnter);
                var existingPevent = await _parkingEventsRepository.GetByIdAsync(parkingEvent.Id, _unitOfWork.Context);
                if (existingPevent != null)
                {
                    await _parkingEventsRepository.UpdateAsync(existingPevent, _unitOfWork.Context);
                }
                else
                {
                    await _parkingEventsRepository.AddAsync(parkingEvent, _unitOfWork.Context);
                }
            }
        }

        private async Task Close(ParkingEvents parkingEvent, DateTime date, ParkingEventClosingReason reason)
        {
            if (reason == ParkingEventClosingReason.Undefined)
            {
                throw new ArgumentException("Invalid closing reason", nameof(reason));
            }

            parkingEvent.Exit = date;
            parkingEvent.ClosingReason = (int)reason;
        }

        public void AssignEnterExitState(ParkingEvents parkingEvent, ParkingEventDirection direction, DateTime now, ParkingEventClosingReason reason)
        {
            if (Direction(parkingEvent) == ParkingEventDirection.Undefined)
            {
                if (direction == ParkingEventDirection.Enter)
                {
                    parkingEvent.Enter = now;
                }
                else
                {
                    parkingEvent.Exit = now;
                }
            }
            else
            {
                if (direction == ParkingEventDirection.Exit)
                {
                    if (reason == ParkingEventClosingReason.Undefined)
                    {
                        throw new Exception("Closing reason was not set.");
                    }

                    parkingEvent.ClosingReason = (int)reason;
                    parkingEvent.Exit = now;
                }
                else
                {
                    throw new Exception("State direction is set to enter, but stored event is already in enter state.");
                }
            }
        }

        public void AddParkingEvent(Installations installation, string eventType)
        {
            AddParkingEvent(installation.Id, eventType);
        }

        public void AddParkingEvent(string installationId, string eventType)
        {
            _parkingEvents.Add(
                1,
                new("parking_event_type", eventType),
                new("installation_id", installationId)
            );
        }

        private async Task<string> HandleDuplicated(ParkingEvents parkingEvent, CreateVehicleParkingRequest request)
        {
            _logger.LogInformation("Duplicated vehicle {direction} parking event {event}", request.Direction, parkingEvent.Id);
            if (parkingEvent.ParkingRight is null)
            {
                return null;
            }

            var parkingRight = await _parkingRightsRepository.GetByValidPlateAsync(parkingEvent.Plate, request.Device.Date, parkingEvent.Installation.ConfigurationParkingRightMatchOffset, _unitOfWork.Context);
            if (parkingRight is null)
            {
                return null;
            }

            parkingEvent.ParkingRight = parkingRight;
            parkingEvent.Tariff = parkingRight.Tariff;

            if (string.IsNullOrWhiteSpace(parkingEvent.Plate))
            {
                return null;
            }

            parkingEvent.Plate = GetBestPlate(parkingRight.Plates, parkingEvent);
            parkingEvent.PlateConfidence = 1;

            await _parkingEventsRepository.UpdateAsync(parkingEvent, _unitOfWork.Context);
            return parkingEvent.Id;
        }

        private string GetBestPlate(List<string>? plates, ParkingEvents parkingEvent)
        {
            var plate = parkingEvent.Plate!;

            if (plates is null)
            {
                return plate;
            }

            if (plates.Count == 1)
            {
                return plates[0];
            }

            var lv = new Levenshtein(plate);
            string? bestPlate = null;
            var bestDistance = int.MaxValue;

            foreach (var current in plates)
            {
                var distance = lv.DistanceFrom(current);

                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestPlate = current;
            }

            return bestPlate ?? plate;
        }

        private async Task<ParkingEvents> FindDuplicateByPlate(CreateVehicleParkingRequest request)
        {
            var duplicated = await _vechicleEventsRepository.GetDuplicateByPlateAndOrDirectionAndOrDate(request.Vehicle.Plate, request.Direction, request.Device.Date, _unitOfWork.Context);

            VehicleEvents vehicleEventDb = null;
            switch (duplicated.Count)
            {
                case 0:
                    break;

                case 1:
                    vehicleEventDb = duplicated[0];
                    break;

                default:
                    for (var i = 0; i < duplicated.Count - 1; ++i)
                    {
                        await _vechicleEventsRepository.RemoveAsync(duplicated[i], _unitOfWork.Context);
                    }

                    vehicleEventDb = duplicated.Last();
                    break;
            }

            if (vehicleEventDb is null)
            {
                return null;
            }

            if (vehicleEventDb.ConfidencePlate < request.Vehicle.Confidence?.Plate)
            {
                vehicleEventDb.ConfidencePlate = request.Vehicle.Confidence?.Plate;
                vehicleEventDb.Plate = request.Vehicle.Plate;
            }

            var parkingEvent = await _parkingEventsRepository.GetByIdAsync(vehicleEventDb.Id, _unitOfWork.Context);

            if (parkingEvent is null)
            {
                return null;
            }

            if (parkingEvent.PlateConfidence is null || parkingEvent.PlateConfidence < vehicleEventDb.ConfidencePlate)
            {
                parkingEvent.PlateConfidence = vehicleEventDb.ConfidencePlate;
                parkingEvent.Plate = vehicleEventDb.Plate;

                if (request.Direction == (int)ParkingEventDirection.Exit)
                {
                    await _parkingEventsRepository.UpdateAsync(parkingEvent, _unitOfWork.Context);
                }
            }

            return parkingEvent;
        }
    }
}
