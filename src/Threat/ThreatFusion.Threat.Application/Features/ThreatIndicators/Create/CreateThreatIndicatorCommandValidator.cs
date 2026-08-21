using FluentValidation;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Create;

public sealed class CreateThreatIndicatorCommandValidator
    : AbstractValidator<CreateThreatIndicatorCommand>
{
    public CreateThreatIndicatorCommandValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Indicator value is required.");

        RuleFor(x => x.SourceName)
            .NotEmpty()
            .WithMessage("Source name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Confidence)
            .InclusiveBetween(0, 100);

        RuleFor(x => x)
            .Must(HaveValidIndicatorValue)
            .WithMessage(
                "Indicator value is not valid for the selected indicator type.");
    }

    private static bool HaveValidIndicatorValue(
        CreateThreatIndicatorCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Value))
        {
            return false;
        }

        var value =
            command.Value.Trim();

        return command.Type switch
        {
            IndicatorType.IpAddress =>
                IPAddress.TryParse(
                    value,
                    out _),

            IndicatorType.Domain =>
                IsValidDomain(value),

            IndicatorType.Url =>
                Uri.TryCreate(
                    value,
                    UriKind.Absolute,
                    out var uri)
                &&
                (
                    uri.Scheme == Uri.UriSchemeHttp ||
                    uri.Scheme == Uri.UriSchemeHttps
                ),

            IndicatorType.Email =>
                IsValidEmail(value),

            IndicatorType.FileHash =>
                IsValidHash(value),

            IndicatorType.Cve =>
                Regex.IsMatch(
                    value,
                    @"^CVE-\d{4}-\d{4,}$",
                    RegexOptions.IgnoreCase),

            _ => false
        };
    }

    private static bool IsValidDomain(
        string value)
    {
        var normalizedDomain =
            value
                .Trim()
                .TrimEnd('.');
    
        if (normalizedDomain.Length == 0 ||
            normalizedDomain.Length > 253)
        {
            return false;
        }
    
        return Regex.IsMatch(
            normalizedDomain,
            @"^(?!-)(?:[A-Za-z0-9-]{1,63}\.)+[A-Za-z]{2,63}$");
    }

    private static bool IsValidEmail(
        string value)
    {
        try
        {
            var address =
                new MailAddress(value);

            return address.Address ==
                   value;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidHash(
        string value)
    {
        return Regex.IsMatch(
            value,
            @"^[A-Fa-f0-9]{32}$")
        ||
        Regex.IsMatch(
            value,
            @"^[A-Fa-f0-9]{40}$")
        ||
        Regex.IsMatch(
            value,
            @"^[A-Fa-f0-9]{64}$");
    }
}