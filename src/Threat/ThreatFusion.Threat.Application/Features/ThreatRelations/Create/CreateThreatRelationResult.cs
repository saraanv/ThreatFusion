namespace ThreatFusion.Threat.Application.Features.ThreatRelations.Create;

public sealed record CreateThreatRelationResult(
    bool IsSuccess,
    long? RelationId,
    IReadOnlyCollection<string> Errors)
{
    public static CreateThreatRelationResult Success(
        long relationId) =>
        new(
            true,
            relationId,
            Array.Empty<string>());

    public static CreateThreatRelationResult Failure(
        params string[] errors) =>
        new(
            false,
            null,
            errors);
}