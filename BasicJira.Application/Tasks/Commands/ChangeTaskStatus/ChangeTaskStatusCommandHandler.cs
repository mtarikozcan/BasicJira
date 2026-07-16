using BasicJira.Application.Common.Interfaces;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.ChangeTaskStatus;

public class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTaskStatusCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);

        if (task == null)
            throw new Exception("Task not found.");

        task.Status = request.Status;

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
