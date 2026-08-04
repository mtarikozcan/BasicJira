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
    public async Task<IActionResult> Create(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var commentId = await _mediator.Send(command, cancellationToken);

        return Ok(commentId);
    }

    [HttpGet("task/{taskId:guid}")]
    public async Task<IActionResult> GetByTask(Guid taskId, CancellationToken cancellationToken)
    {
        var query = new GetCommentsByTaskQuery(taskId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command with { Id = id }, cancellationToken);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCommentCommand(id), cancellationToken);

        return Ok();
    }
}
