using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Enums;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.ChangeTaskPriority;

public record ChangeTaskPriorityCommand(
    Guid TaskId,
    TaskPriority Priority
) : IRequest;
