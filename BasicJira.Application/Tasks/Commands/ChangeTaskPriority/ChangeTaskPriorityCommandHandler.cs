using BasicJira.Application.Common.Interfaces;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.ChangeTaskPriority;

public class ChangeTaskPriorityCommandHandler : IRequestHandler<ChangeTaskPriorityCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTaskPriorityCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeTaskPriorityCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);

        if (task == null)
            throw new Exception("Task not found.");

        task.Priority = request.Priority;

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
