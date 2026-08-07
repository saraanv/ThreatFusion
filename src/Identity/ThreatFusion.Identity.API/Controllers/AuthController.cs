using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Identity.Application.Features.Authentication.Register;
using ThreatFusion.Identity.Application.Features.Authentication.Login;

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
}