using BasicJira.Application.Comments.Commands.CreateComment;
using BasicJira.Application.Comments.Commands.DeleteComment;
using BasicJira.Application.Comments.Commands.UpdateComment;
using BasicJira.Application.Comments.Queries.GetCommentsByTask;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentCommand command)
    {
        var commentId = await _mediator.Send(command);

        return Ok(commentId);
    }

    [HttpGet("task/{taskId:guid}")]
    public async Task<IActionResult> GetByTask(Guid taskId)
    {
        var query = new GetCommentsByTaskQuery(taskId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCommentCommand command)
    {
        await _mediator.Send(command with { Id = id });

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCommentCommand(id));

        return Ok();
    }
}
