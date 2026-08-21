namespace ThreatFusion.Threat.Application.Features.Alerts.GetMine;

public sealed record ThreatAlertDto(
    long Id,
    long ThreatIndicatorId,
    string IndicatorValue,
    string AlertType,
    string Title,
    string Message,
    string Severity,
    bool IsRead,
    DateTime CreatedAtUtc,
    DateTime? ReadAtUtc);