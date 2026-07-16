using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Enums;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(
    Guid Id,
    Guid ProjectId,
    Guid? AssignedUserId,
    string Title,
    string? Description,
    TaskPriority Priority,
    TaskItemStatus Status
) : IRequest;
