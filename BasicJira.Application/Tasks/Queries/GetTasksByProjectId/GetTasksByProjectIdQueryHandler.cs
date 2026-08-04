using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.DTOs;
using MediatR;

namespace BasicJira.Application.Tasks.Queries.GetTasksByProjectId;

public class GetTasksByProjectIdQueryHandler : IRequestHandler<GetTasksByProjectIdQuery, List<TaskDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;

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
            throw new Exception("Project not found.");

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
