using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.Common.Authorization;
using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, List<TaskDto>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTasksQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TaskItems
            .AsNoTracking()
            .AsQueryable();

        if (_currentUserService.Role != Roles.Admin)
        {
            var currentUserId = _currentUserService.UserId
                ?? throw new ForbiddenException();

            query = query.Where(task =>
                _context.ProjectMembers.Any(member =>
                    member.ProjectId == task.ProjectId &&
                    member.UserId == currentUserId));
        }

        return await query
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
