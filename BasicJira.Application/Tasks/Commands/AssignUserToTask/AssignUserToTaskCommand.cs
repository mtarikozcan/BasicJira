using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Tasks.Commands.AssignUserToTask;

public record AssignUserToTaskCommand(
    Guid TaskId,
    Guid UserId
) : IRequest;
