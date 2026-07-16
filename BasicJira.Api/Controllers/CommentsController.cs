using BasicJira.Application.Comments.Commands.CreateComment;
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
}
