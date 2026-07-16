using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FullName,
    string Email,
    string Role
) : IRequest<Guid>;
