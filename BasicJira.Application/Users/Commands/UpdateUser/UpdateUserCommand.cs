using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string FullName,
    string Email,
    string Role
) : IRequest;
