using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces; // IAppDbContext için gerekli
using BasicJira.Application.Common.Authorization;
using BasicJira.Application.DTOs;
using BasicJira.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskByIdQueryHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        // Yalnızca okuma yapacağımız için performans artışı adına AsNoTracking() kullanıyoruz
        var task = await _context.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (task == null)
        {
            throw new NotFoundException(nameof(TaskItem), request.Id);
        }

        if (_currentUserService.Role != Roles.Admin)
        {
            var currentUserId = _currentUserService.UserId
                ?? throw new ForbiddenException();

            var isMember = await _context.ProjectMembers
                .AsNoTracking()
                .AnyAsync(
                    member =>
                        member.ProjectId == task.ProjectId &&
                        member.UserId == currentUserId,
                    cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException(
                    "You are not a member of the project that contains this task.");
            }
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