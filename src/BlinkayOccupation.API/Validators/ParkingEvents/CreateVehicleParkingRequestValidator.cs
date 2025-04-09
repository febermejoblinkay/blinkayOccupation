using BlinkayOccupation.Application.Models;
using FluentValidation;

namespace BlinkayOccupation.API.Validators.Stays
{
    public class CreateVehicleParkingRequestValidator : AbstractValidator<CreateVehicleParkingRequest>
    {
        public CreateVehicleParkingRequestValidator()
        {
            RuleFor(x => x.InstallationId)
                .NotEmpty().WithMessage("InstallationId is required.");

            RuleFor(x => x.Device)
                .NotNull().WithMessage("Device information is required.");

            When(x => x.Device != null, () =>
            {
                RuleFor(x => x.Device.Id)
                    .NotEmpty().WithMessage("Device Id is required.");
                RuleFor(x => x.Device.Date)
                    .NotEmpty().WithMessage("Device Date is required.");
                RuleFor(x => x.Device.Inserted)
                    .NotEmpty().WithMessage("Device Inserted is required.");
            });

            RuleFor(x => x.ParkingArea)
                .NotNull().WithMessage("ParkingArea information is required.");

            When(x => x.ParkingArea != null, () =>
            {
                RuleFor(x => x.ParkingArea.ZoneId)
                    .NotEmpty().WithMessage("ZoneId is required.");
            });

            RuleFor(x => x.Vehicle)
                .NotNull().WithMessage("Vehicle information is required.");

            When(x => x.Vehicle != null, () =>
            {
                RuleFor(x => x.Vehicle.Plate)
                .Must((plate) => !string.Equals(plate, "NOPLATE", StringComparison.Ordinal))
                .WithMessage("Plate must be NOPLATE.");

                //RuleFor(x => x.Vehicle.Make)
                //    .NotEmpty().WithMessage("Vehicle Make is required.");

                //RuleFor(x => x.Vehicle.Model)
                //    .NotEmpty().WithMessage("Vehicle Model is required.");

                //RuleFor(x => x.Vehicle.Color)
                //    .NotEmpty().WithMessage("Vehicle Color is required.");

                //RuleFor(x => x.Vehicle.Nationality)
                //    .NotEmpty().WithMessage("Vehicle Nationality is required.");

                //RuleFor(x => x.Vehicle.Confidence)
                //    .NotNull().WithMessage("Vehicle Confidence is required.");
            });

            RuleFor(x => x.Direction)
                .InclusiveBetween(0, 2).WithMessage("Direction must be 1 or 2.");
            
            RuleFor(x => x.Direction)
            .Must(direction => (ParkingEventDirection)direction == ParkingEventDirection.Enter || (ParkingEventDirection)direction == ParkingEventDirection.Exit)
            .WithMessage("Direction must be Enter or Exit.");
        }
    }
}
