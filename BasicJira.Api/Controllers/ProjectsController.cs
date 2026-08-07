using BasicJira.Application.Projects.Commands.CreateProject;
using BasicJira.Application.Projects.Commands.DeleteProject;
using BasicJira.Application.Projects.Commands.UpdateProject;
using BasicJira.Application.Projects.Queries.GetProjectById;
using BasicJira.Application.Projects.Queries.GetProjects;
using BasicJira.Application.Common.Authorization;
using BasicJira.Application.Projects.Commands.AddProjectMember;
using BasicJira.Application.Projects.Commands.RemoveProjectMember;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BasicJira.Application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var projectId = await _mediator.Send(command, cancellationToken);

        return Ok(projectId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var projects = await _mediator.Send(new GetProjectsQuery(), cancellationToken);
        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProjectByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command with { Id = id }, cancellationToken);

        return Ok();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProjectCommand(id), cancellationToken);

        return Ok();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{projectId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> AddMember(
    Guid projectId,
    Guid userId,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new AddProjectMemberCommand(projectId, userId),
            cancellationToken);

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{projectId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RemoveProjectMemberCommand(projectId, userId),
            cancellationToken);

        return NoContent();
    }

}


// Thin Controller, Fat Handler
// controller request alıp mediatore gönder. response dondur. controllerda business logic yok. handlerda var.


// aslında şunu yakalamak istiyoruz. http request -> projects controller -> IMediator.Send() -> CreateProjectCommandHandler -> IAppDbContext -> AppDbContext -> SQL server. 
