using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Services;

public static class ThreatRiskCalculator
{
    public static (double Score, ThreatRiskLevel Level) Calculate(
        ThreatSeverity severity,
        int confidence,
        double? cvssScore,
        string sourceName)
    {
        double score = 0;

        // CVSS → maximum 40 points
        if (cvssScore.HasValue)
        {
            score += cvssScore.Value * 4;
        }

        // Severity → maximum 30 points
        score += severity switch
        {
            ThreatSeverity.Low => 5,
            ThreatSeverity.Medium => 10,
            ThreatSeverity.High => 20,
            ThreatSeverity.Critical => 30,
            _ => 0
        };

        // Confidence → maximum 20 points
        score += confidence * 0.2;

        // Trusted / exploited source bonus
        if (string.Equals(
                sourceName,
                "CISA-KEV",
                StringComparison.OrdinalIgnoreCase))
        {
            score += 10;
        }

        score = Math.Min(score, 100);

        var level = score switch
        {
            >= 80 => ThreatRiskLevel.Critical,
            >= 60 => ThreatRiskLevel.High,
            >= 30 => ThreatRiskLevel.Medium,
            _ => ThreatRiskLevel.Low
        };

        return (
            Math.Round(score, 2),
            level);
    }
}