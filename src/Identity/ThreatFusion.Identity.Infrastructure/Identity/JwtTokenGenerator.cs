using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ThreatFusion.Identity.Application.Abstractions;
using ThreatFusion.Identity.Application.Common.Models;

namespace ThreatFusion.Identity.Infrastructure.Identity;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(
        IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public Task<TokenResult> GenerateAsync(
        long userId,
        string email,
        string firstName,
        string lastName,
        IEnumerable<string> roles)
    {
        var expiresAtUtc =
            DateTime.UtcNow.AddMinutes(
                _options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                email),

            new(
                JwtRegisteredClaimNames.GivenName,
                firstName),

            new(
                JwtRegisteredClaimNames.FamilyName,
                lastName),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        claims.AddRange(
            roles.Select(role =>
                new Claim(ClaimTypes.Role, role)));

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.Key));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return Task.FromResult(
            new TokenResult(
                accessToken,
                expiresAtUtc));
    }
}