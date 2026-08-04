using BasicJira.Application.Common.Interfaces;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.UnassignUserFromTask;

public class UnassignUserFromTaskCommandHandler : IRequestHandler<UnassignUserFromTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnassignUserFromTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UnassignUserFromTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);

        if (task == null)
            throw new Exception("Task not found.");

        task.AssignedUserId = null;

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
