using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.Common.Authorization;
using BasicJira.Application.DTOs;
using BasicJira.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Comments.Queries.GetCommentsByTask;

public class GetCommentsByTaskQueryHandler : IRequestHandler<GetCommentsByTaskQuery, List<CommentDto>>
{
    private readonly IAppDbContext _context;
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCommentsByTaskQueryHandler(
        IAppDbContext context,
        ITaskRepository taskRepository,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskItemId, cancellationToken);

        if (task == null)
            throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

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

        return await _context.TaskComments
            .AsNoTracking()
            .Where(comment => comment.TaskItemId == request.TaskItemId)
            .Select(comment => new CommentDto
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                UserId = comment.UserId,
                Comment = comment.Comment,
                CreatedAt = comment.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
