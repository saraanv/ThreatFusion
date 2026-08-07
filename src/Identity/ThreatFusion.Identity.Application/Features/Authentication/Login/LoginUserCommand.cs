using MediatR;
using ThreatFusion.Identity.Application.Common.Models;

namespace ThreatFusion.Identity.Application.Features.Authentication.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResult>;