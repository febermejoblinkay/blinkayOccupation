using BlinkayOccupation.Application.Models;
using FluentValidation;

namespace BlinkayOccupation.API.Validators.Stays
{
    public class UpdateStayRequestValidator : AbstractValidator<UpdateStayRequest>
    {
        public UpdateStayRequestValidator()
        {
            RuleFor(x => x.StayId).NotNull().NotEmpty().WithMessage("StayId should not be null or empty");
            RuleFor(x => x.EntryDate)
                .Must(date => !date.HasValue || DateTime.TryParse(date.ToString(), out _))
                .WithMessage("The EntryDate should be a valid date.")
                .When(x => x.EntryDate.HasValue)
                .NotNull()
                .WithMessage("EntryDate is required when EntryEventId is present.")
                .When(x => !string.IsNullOrEmpty(x.EntryEventId));

            RuleFor(x => x.ExitDate)
                .Must(date => !date.HasValue || DateTime.TryParse(date.ToString(), out _))
                .WithMessage("The ExitDate should be a valid date.")
                .When(x => x.ExitDate.HasValue)
                .NotNull()
                .WithMessage("ExitDate is required when ExitEventId is present.")
                .When(x => !string.IsNullOrEmpty(x.ExitEventId));
            When(x => x.EntryDate.HasValue && x.ExitDate.HasValue, () =>
            {
                RuleFor(x => x.EntryDate.Value)
                    .LessThan(x => x.ExitDate.Value)
                    .WithMessage("EntryDate should be smaller than ExitDate.");

                RuleFor(x => x.ExitDate.Value)
                    .GreaterThan(x => x.EntryDate.Value)
                    .WithMessage("ExitDate should be greater than EntryDate.");
            });
            When(x => x.ParkingRightIds?.Count > 0, () =>
            {
                RuleFor(x => x.ParkingRightIds).NotEmpty().NotNull().WithMessage("ParkingRightIds are in incorrect format");
            });
            RuleFor(x => x.CaseId).Must(x => x.HasValue && x.Value != 0).WithMessage("CaseId should not be null or 0");
        }
    }
}
