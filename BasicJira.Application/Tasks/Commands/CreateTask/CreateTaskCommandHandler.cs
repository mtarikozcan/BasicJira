using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using BasicJira.Domain.Enums;
using MediatR;

namespace BasicJira.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(
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

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);

        if (!projectExists)
            throw new Exception("Project not found.");

        if (request.AssignedUserId.HasValue)
        {
            var userExists = await _userRepository.ExistsAsync(request.AssignedUserId.Value, cancellationToken);

            if (!userExists)
                throw new Exception("Assigned user not found.");
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            AssignedUserId = request.AssignedUserId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = TaskItemStatus.Todo,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}