using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.Common.Authorization;
using Microsoft.EntityFrameworkCore;
using BasicJira.Application.DTOs;
using BasicJira.Domain.Entities;
using MediatR;

namespace BasicJira.Application.Tasks.Queries.GetTasksByProjectId;

public class GetTasksByProjectIdQueryHandler : IRequestHandler<GetTasksByProjectIdQuery, List<TaskDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTasksByProjectIdQueryHandler(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskDto>> Handle(GetTasksByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);

        if (!projectExists)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        if (_currentUserService.Role != Roles.Admin)
        {
            var currentUserId = _currentUserService.UserId
                ?? throw new ForbiddenException();

            var isMember = await _context.ProjectMembers
                .AsNoTracking()
                .AnyAsync(
                    member =>
                        member.ProjectId == request.ProjectId &&
                        member.UserId == currentUserId,
                    cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException("You are not a member of this project.");
            }
        }

        var tasks = await _taskRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        return tasks.Select(task => new TaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            AssignedUserId = task.AssignedUserId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            CreatedAt = task.CreatedAt
        }).ToList();
    }
}
