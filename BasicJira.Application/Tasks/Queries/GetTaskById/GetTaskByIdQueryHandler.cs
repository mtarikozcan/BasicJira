using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces; // IAppDbContext için gerekli
using BasicJira.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly IAppDbContext _context;

    public GetTaskByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        // Yalnızca okuma yapacağımız için performans artışı adına AsNoTracking() kullanıyoruz
        var task = await _context.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (task == null)
        {
           
            throw new Exception("Task not found.");
        }

        // Entity'den DTO'na map'leme işlemi
        return new TaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            AssignedUserId = task.AssignedUserId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            CreatedAt = task.CreatedAt
        };
    }
}