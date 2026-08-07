using Microsoft.AspNetCore.Identity;
using ThreatFusion.Identity.Application.Abstractions;
using ThreatFusion.Identity.Application.Common.Models;
using ThreatFusion.Identity.Domain.Entities;

namespace ThreatFusion.Identity.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterUserResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();

        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);

        if (existingUser is not null)
        {
            return RegisterUserResult.Failure(["A user with this email already exists."]);
        }

        var now = DateTime.UtcNow;

        var user = new ApplicationUser
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
            CreatedAtUtc = now,
            RegDate = int.Parse(now.ToString("yyyyMMdd")),
            RegTime = now.ToString("HH:mm"),
            IsActive = true,
            IsDeleted = false
        };

        var identityResult = await _userManager.CreateAsync(user, password);

        if (!identityResult.Succeeded)
        {
            return RegisterUserResult.Failure(identityResult.Errors.Select(error => error.Description));
        }

        return RegisterUserResult.Success(user.Id);
    }
}