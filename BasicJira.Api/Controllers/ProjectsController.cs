using BasicJira.Application.Projects.CreateProject;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasicJira.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command)
    {
        var projectId = await _mediator.Send(command);

        return Ok(projectId);
    }


}


// Thin Controller, Fat Handler
// controller request alıp mediatore gönder. response dondur. controllerda business logic yok. handlerda var.


// aslında şunu yakalamak istiyoruz. http request -> projects controller -> IMediator.Send() -> CreateProjectCommandHandler -> IAppDbContext -> AppDbContext -> SQL server. 

