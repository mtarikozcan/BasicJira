using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.AssignUserToTask;

public class AssignUserToTaskCommandHandler : IRequestHandler<AssignUserToTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignUserToTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignUserToTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);

        if (task == null)
            throw new NotFoundException(nameof(TaskItem), request.TaskId);

        var userExists = await _userRepository.ExistsAsync(request.UserId, cancellationToken);

        if (!userExists)
            throw new NotFoundException(nameof(AppUser), request.UserId);

        if (task.AssignedUserId == request.UserId)
            throw new ConflictException("User is already assigned to this task.");

        task.AssignedUserId = request.UserId;

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
