using MediatR;

namespace BasicJira.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
    ) : IRequest<string>;