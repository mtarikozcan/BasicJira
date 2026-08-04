using BasicJira.Application.Tasks.Commands.AssignUserToTask;
using BasicJira.Application.Tasks.Commands.ChangeTaskPriority;
using BasicJira.Application.Tasks.Commands.ChangeTaskStatus;
using BasicJira.Application.Tasks.Commands.CreateTask;
using BasicJira.Application.Tasks.Commands.DeleteTask;
using BasicJira.Application.Tasks.Commands.UnassignUserFromTask;
using BasicJira.Application.Tasks.Commands.UpdateTask;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BasicJira.Application.Tasks.Queries.GetTaskById;
using BasicJira.Application.Tasks.Queries.GetTasks;
using BasicJira.Application.Tasks.Queries.GetTasksByProjectId;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var taskId = await _mediator.Send(command, cancellationToken);

        return Ok(taskId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tasks = await _mediator.Send(new GetTasksQuery(), cancellationToken);

        return Ok(tasks);
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetByProjectId(Guid projectId, CancellationToken cancellationToken)
    {
        var query = new GetTasksByProjectIdQuery(projectId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTaskByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command with { Id = id }, cancellationToken);

        return Ok();
    }

    [HttpPut("{id:guid}/assign-user/{userId:guid}")]
    public async Task<IActionResult> AssignUser(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new AssignUserToTaskCommand(id, userId), cancellationToken);

        return Ok();
    }

    [HttpPut("{id:guid}/priority")]
    public async Task<IActionResult> ChangePriority(Guid id, ChangeTaskPriorityCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command with { TaskId = id }, cancellationToken);

        return Ok();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeTaskStatusCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command with { TaskId = id }, cancellationToken);

        return Ok();
    }

    [HttpPut("{id:guid}/unassign-user")]
    public async Task<IActionResult> UnassignUser(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UnassignUserFromTaskCommand(id), cancellationToken);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTaskCommand(id), cancellationToken);

        return Ok();
    }
}
