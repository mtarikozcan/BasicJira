using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
