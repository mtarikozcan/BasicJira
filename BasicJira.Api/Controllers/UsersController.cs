using BasicJira.Application.Users.Commands.CreateUser;
using BasicJira.Application.Users.Commands.DeleteUser;
using BasicJira.Application.Users.Commands.UpdateUser;
using BasicJira.Application.Users.Queries.GetUserById;
using BasicJira.Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BasicJira.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command, cancellationToken);

        return Ok(userId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new GetUsersQuery(), cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command with { Id = id }, cancellationToken);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteUserCommand(id), cancellationToken);

        return Ok();
    }
}
