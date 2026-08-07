using FluentValidation;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;

public sealed class CreateThreatIndicatorCommandValidator
    : AbstractValidator<CreateThreatIndicatorCommand>
{
    public CreateThreatIndicatorCommandValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(2048);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Severity)
            .IsInEnum();

        RuleFor(x => x.Confidence)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.SourceName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.LastSeenUtc)
            .GreaterThanOrEqualTo(x => x.FirstSeenUtc!.Value)
            .When(x =>
                x.FirstSeenUtc.HasValue &&
                x.LastSeenUtc.HasValue);
    }
}