namespace ThreatFusion.Threat.API.Models;

public sealed record ApiErrorResponse(
    string ErrorCode,
    string Message,
    IReadOnlyCollection<string>? Errors,
    string TraceId);