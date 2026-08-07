using BasicJira.Application.Common.Authorization;
using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler
    : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppDbContext _context;

    public CreateCommentCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        ICommentRepository commentRepository,
        IUnitOfWork unitOfWork,
        IAppDbContext context,
        ICurrentUserService currentUserService)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException(
                "Authenticated user information could not be found.");

        var task = await _taskRepository.GetByIdAsync(
            request.TaskItemId,
            cancellationToken);

        if (task is null)
        {
            throw new NotFoundException(
                nameof(TaskItem),
                request.TaskItemId);
        }

        if (_currentUserService.Role != Roles.Admin)
        {
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

        var userExists = await _userRepository.ExistsAsync(
            currentUserId,
            cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException(
                nameof(AppUser),
                currentUserId);
        }

        var comment = new TaskComment
        {
            Id = Guid.NewGuid(),
            TaskItemId = request.TaskItemId,
            UserId = currentUserId,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(
            comment,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return comment.Id;
    }
}