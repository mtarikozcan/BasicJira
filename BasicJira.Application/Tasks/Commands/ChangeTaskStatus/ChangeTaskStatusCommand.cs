using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Enums;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.ChangeTaskStatus;

public record ChangeTaskStatusCommand(
    Guid TaskId,
    TaskItemStatus Status
) : IRequest;
