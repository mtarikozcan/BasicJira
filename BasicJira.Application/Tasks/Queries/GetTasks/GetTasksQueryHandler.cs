using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, List<TaskDto>>
{
    private readonly IAppDbContext _context;

    public GetTasksQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Select(task => new TaskDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                AssignedUserId = task.AssignedUserId,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                Status = task.Status,
                CreatedAt = task.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
