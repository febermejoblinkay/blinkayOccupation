using BlinkayOccupation.Application.Models;
using FluentValidation;

namespace BlinkayOccupation.API.Validators.Stays
{
    public class AddStayRequestValidator : AbstractValidator<AddStayRequest>
    {
        public AddStayRequestValidator()
        {
            RuleFor(x => x.ZoneId)
                .NotEmpty()
                .WithMessage("ZoneId should not be null or empty");

            RuleFor(x => x.InstallationId)
                .NotEmpty()
                .WithMessage("InstallationId should not be null or empty");

            RuleFor(x => x.EntryDate)
                .Must(date => date != DateTime.MinValue)
                .WithMessage("EntryDate is not valid.")
                .When(x => x.EntryDate.HasValue);

            RuleFor(x => x.ExitDate)
                .Must(date => date != DateTime.MinValue)
                .WithMessage("ExitDate is not valid.")
                .When(x => x.ExitDate.HasValue);

            RuleFor(x => x.EntryDate)
                .NotNull()
                .WithMessage("EntryDate is required when EntryEventId is present.")
                .When(x => !string.IsNullOrEmpty(x.EntryEventId));

            RuleFor(x => x.EntryEventId)
                .NotEmpty()
                .WithMessage("EntryEventId is required when EntryDate is present.")
                .When(x => x.EntryDate.HasValue);

            RuleFor(x => x.ExitDate)
                .NotNull()
                .WithMessage("ExitDate is required when ExitEventId is present.")
                .When(x => !string.IsNullOrEmpty(x.ExitEventId));

            RuleFor(x => x.ExitEventId)
                .NotEmpty()
                .WithMessage("ExitEventId is required when ExitDate is present.")
                .When(x => x.ExitDate.HasValue);

            When(x => x.EntryDate.HasValue && x.ExitDate.HasValue, () =>
            {
                RuleFor(x => x.EntryDate.Value)
                    .LessThan(x => x.ExitDate.Value)
                    .WithMessage("EntryDate should be smaller than ExitDate.");
            });

            RuleFor(x => x.ParkingRightIds)
                .NotEmpty()
                .WithMessage("ParkingRightIds should not be empty when provided.")
                .When(x => x.ParkingRightIds?.Count > 0);

            RuleFor(x => x.CaseId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("CaseId should not be null or 0");
        }
    }
}
