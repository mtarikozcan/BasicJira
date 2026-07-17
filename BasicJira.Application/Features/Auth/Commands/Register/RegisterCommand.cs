using MediatR;

namespace BasicJira.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Name,
    string Email,
    string Password
) : IRequest<Guid>;