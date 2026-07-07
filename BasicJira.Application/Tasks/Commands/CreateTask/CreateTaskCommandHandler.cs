using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using BasicJira.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var projectExists = await _context.Projects
            .AnyAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (!projectExists)
            throw new Exception("Project not found.");

        if (request.AssignedUserId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(x => x.Id == request.AssignedUserId.Value, cancellationToken);

            if (!userExists)
                throw new Exception("Assigned user not found.");
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            AssignedUserId = request.AssignedUserId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,

            Status = TaskItemStatus.Todo,

            CreatedAt = DateTime.UtcNow 
        };

        _context.TaskItems.Add(task);

        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}