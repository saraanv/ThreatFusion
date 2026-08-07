using Microsoft.AspNetCore.Identity;
using ThreatFusion.Identity.Application.Abstractions;
using ThreatFusion.Identity.Application.Common.Models;
using ThreatFusion.Identity.Application.Features.Users.AssignRole;
using ThreatFusion.Identity.Domain.Constants;
using ThreatFusion.Identity.Domain.Entities;

namespace ThreatFusion.Identity.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
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
        await _userManager.AddToRoleAsync(
            user,
            Roles.Viewer);
        if (!identityResult.Succeeded)
        {
            return RegisterUserResult.Failure(identityResult.Errors.Select(error => error.Description));
        }

        return RegisterUserResult.Success(user.Id);
    }

    public async Task<LoginUserResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();

        var user =
            await _userManager.FindByEmailAsync(
                normalizedEmail);

        if (user is null ||
            user.IsDeleted ||
            !user.IsActive)
        {
            return LoginUserResult.Failure(
                "Invalid email or password.");
        }

        var signInResult =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return LoginUserResult.Failure(
                "The account is temporarily locked.");
        }

        if (!signInResult.Succeeded)
        {
            return LoginUserResult.Failure(
                "Invalid email or password.");
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var token =
            await _jwtTokenGenerator.GenerateAsync(
                user.Id,
                user.Email!,
                user.FirstName,
                user.LastName,
                roles);

        return LoginUserResult.Success(
            token.AccessToken,
            token.ExpiresAtUtc,
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email!);
    }
    
    public async Task<AssignRoleResult> AssignRoleAsync(
        long userId,
        string role,
        CancellationToken cancellationToken)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());
    
        if (user is null ||
            user.IsDeleted)
        {
            return AssignRoleResult.Failure(
                "User was not found.");
        }
    
        string[] validRoles =
        [
            Roles.Admin,
            Roles.Analyst,
            Roles.Viewer
        ];
    
        if (!validRoles.Contains(
                role,
                StringComparer.OrdinalIgnoreCase))
        {
            return AssignRoleResult.Failure(
                "Invalid role.");
        }
    
        var normalizedRole =
            validRoles.First(x =>
                string.Equals(
                    x,
                    role,
                    StringComparison.OrdinalIgnoreCase));
    
        var currentRoles =
            await _userManager.GetRolesAsync(user);
    
        if (currentRoles.Contains(normalizedRole))
        {
            return AssignRoleResult.Success();
        }
    
        var result =
            await _userManager.AddToRoleAsync(
                user,
                normalizedRole);
    
        if (!result.Succeeded)
        {
            return AssignRoleResult.Failure(
                result.Errors
                    .Select(x => x.Description)
                    .ToArray());
        }
    
        return AssignRoleResult.Success();
    }
}