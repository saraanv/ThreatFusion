using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThreatFusion.Identity.Application
    .Features.Authentication.Register;

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
}