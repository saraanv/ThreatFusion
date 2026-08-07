using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Identity.Application.Features.Authentication.Register;
using ThreatFusion.Identity.Application.Features.Authentication.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ThreatFusion.Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Route("RegisterUser")]
    [ActionName("ثبت نام کاربر")]
    public async Task<IActionResult> Register(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                Errors = result.Errors
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                result.UserId,
                Message = "User registered successfully."
            });
    }
    
    [HttpPost]
    [Route("LoginUser")]
    [ActionName("ورود کاربر")]
    public async Task<IActionResult> Login(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _sender.Send(
                command,
                cancellationToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(new
            {
                Errors = result.Errors
            });
        }

        return Ok(new
        {
            result.AccessToken,
            result.ExpiresAtUtc,

            User = new
            {
                result.UserId,
                result.FirstName,
                result.LastName,
                result.Email
            }
        });
    }
    
    [Authorize]
    [HttpGet]
    [Route("Me")]
    [ActionName("دریافت اطلاعات کاربر جاری")]
    public IActionResult Me()
    {
        var userId =
            User.FindFirstValue(
                JwtRegisteredClaimNames.Sub);

        var email =
            User.FindFirstValue(
                JwtRegisteredClaimNames.Email);

        var firstName =
            User.FindFirstValue(
                JwtRegisteredClaimNames.GivenName);

        var lastName =
            User.FindFirstValue(
                JwtRegisteredClaimNames.FamilyName);

        var roles = User.FindAll("role")
            .Select(claim => claim.Value)
            .ToArray();

        return Ok(new
        {
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Roles = roles
        });
    }
    
}