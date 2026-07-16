using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IAppDbContext _context;

    public GetProjectByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (project == null)
        {
            throw new Exception("Project not found.");
        }

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };
    }
}
