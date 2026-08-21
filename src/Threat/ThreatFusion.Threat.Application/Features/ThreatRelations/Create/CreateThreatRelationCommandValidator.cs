using FluentValidation;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.Create;

public sealed class CreateThreatRelationCommandValidator
    : AbstractValidator<CreateThreatRelationCommand>
{
    public CreateThreatRelationCommandValidator()
    {
        RuleFor(x => x.SourceIndicatorId)
            .GreaterThan(0);

        RuleFor(x => x.TargetIndicatorId)
            .GreaterThan(0);

        RuleFor(x => x)
            .Must(x =>
                x.SourceIndicatorId !=
                x.TargetIndicatorId)
            .WithMessage(
                "Source and target indicators cannot be the same.");

        RuleFor(x => x.Confidence)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}