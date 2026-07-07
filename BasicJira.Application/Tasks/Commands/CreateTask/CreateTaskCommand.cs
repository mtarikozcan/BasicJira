using System;
using BasicJira.Domain.Enums;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    Guid ProjectId,         // pk
    Guid? AssignedUserId,   // fk ama nullable olabilir 
    string Title,           // client tarafında girilmeli, zorunlu
    string? Description,    // client tarafında girilmeli, opsiyonel
    TaskPriority Priority   // default almak yerine, clienttan al
) : IRequest<Guid>;
