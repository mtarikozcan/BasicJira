using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;
using BasicJira.Application.Common.Authorization;

namespace BasicJira.Application.Tasks.Commands.ChangeTaskStatus;

public class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public ChangeTaskStatusCommandHandler(
    IUnitOfWork unitOfWork,
    ITaskRepository taskRepository,
    ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);

        if (task == null)
            throw new NotFoundException(nameof(TaskItem), request.TaskId);

        if (_currentUserService.Role != Roles.Admin)
        {
            var currentUserId = _currentUserService.UserId
                ?? throw new ForbiddenException();

            if (task.AssignedUserId != currentUserId)
            {
                throw new ForbiddenException(
                    "You can only change the status of tasks assigned to you.");
            }
        }

        task.Status = request.Status;

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
