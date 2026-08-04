using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Comments.Queries.GetCommentsByTask;

public class GetCommentsByTaskQueryHandler : IRequestHandler<GetCommentsByTaskQuery, List<CommentDto>>
{
    private readonly IAppDbContext _context;
    private readonly ITaskRepository _taskRepository;

    public GetCommentsByTaskQueryHandler(
        IAppDbContext context,
        ITaskRepository taskRepository)
    {
        _context = context;
        _taskRepository = taskRepository;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskItemId, cancellationToken);

        if (task == null)
            throw new Exception("Task not found.");

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
