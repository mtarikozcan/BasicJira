using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
            throw new NotFoundException(nameof(TaskItem), request.Id);

        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);

        if (!projectExists)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        if (request.AssignedUserId.HasValue)
        {
            var userExists = await _userRepository.ExistsAsync(request.AssignedUserId.Value, cancellationToken);

            if (!userExists)
                throw new NotFoundException(nameof(AppUser), request.AssignedUserId.Value);
        }

        task.ProjectId = request.ProjectId;
        task.AssignedUserId = request.AssignedUserId;
        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.Status = request.Status;

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
