using MediatR;
using ThreatFusion.Identity.Application.Common.Models;

namespace ThreatFusion.Identity.Application.Features.Authentication.Register;

public sealed record RegisterUserCommand(string? FirstName, string? LastName, string? Email, string? Password) : IRequest<RegisterUserResult>;