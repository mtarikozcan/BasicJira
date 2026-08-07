using BasicJira.Application.Common.Authorization;
using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCommentCommandHandler(
        ICommentRepository commentRepository,
        IUnitOfWork unitOfWork,
        IAppDbContext context,
        ICurrentUserService currentUserService)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (comment == null)
            throw new NotFoundException(nameof(TaskComment), request.Id);

        if (_currentUserService.Role != Roles.Admin)
        {
            var currentUserId = _currentUserService.UserId
                ?? throw new ForbiddenException();

            if (comment.UserId != currentUserId)
            {
                throw new ForbiddenException(
                    "You can only delete your own comments.");
            }

            var projectId = await _context.TaskItems
                .AsNoTracking()
                .Where(task => task.Id == comment.TaskItemId)
                .Select(task => task.ProjectId)
                .FirstAsync(cancellationToken);

            var isMember = await _context.ProjectMembers
                .AsNoTracking()
                .AnyAsync(
                    member =>
                        member.ProjectId == projectId &&
                        member.UserId == currentUserId,
                    cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException(
                    "You are no longer a member of this project.");
            }
        }

        _commentRepository.Remove(comment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
