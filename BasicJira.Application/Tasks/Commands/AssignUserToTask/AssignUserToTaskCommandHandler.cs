using BasicJira.Application.Common.Interfaces;
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
            throw new Exception("Task not found.");

        var userExists = await _userRepository.ExistsAsync(request.UserId, cancellationToken);

        if (!userExists)
            throw new Exception("User not found.");

        task.AssignedUserId = request.UserId;

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
