using MediatR;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.ThreatRelations.Create;

public sealed record CreateThreatRelationCommand(
    long SourceIndicatorId,
    long TargetIndicatorId,
    ThreatRelationType RelationType,
    string? Description,
    double Confidence)
    : IRequest<CreateThreatRelationResult>;