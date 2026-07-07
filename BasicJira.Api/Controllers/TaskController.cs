using BasicJira.Application.Tasks.Commands.CreateTask;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BasicJira.Application.Tasks.Queries.GetTaskById;

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
    public async Task<IActionResult> Create(CreateTaskCommand command)
    {
        var taskId = await _mediator.Send(command);

        return Ok(taskId);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetTaskByIdQuery(id);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}