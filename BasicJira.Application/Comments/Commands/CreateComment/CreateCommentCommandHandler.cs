using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;

namespace BasicJira.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommentCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        ICommentRepository commentRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskItemId, cancellationToken);

        if (task == null)
            throw new Exception("Task not found.");

        var userExists = await _userRepository.ExistsAsync(request.UserId, cancellationToken);

        if (!userExists)
            throw new Exception("User not found.");

        var comment = new TaskComment
        {
            Id = Guid.NewGuid(),
            TaskItemId = request.TaskItemId,
            UserId = request.UserId,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
