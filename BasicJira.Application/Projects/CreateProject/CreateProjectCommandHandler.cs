using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;

namespace BasicJira.Application.Projects.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateProjectCommandHandler(IAppDbContext context) 
    { 
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken
        )
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTime.UtcNow // request içinde olmadığı için şu anki zamanı atadık, request.CreatedAt  ile handler içerisinde çağırmak istediğinde compiler error veriyor.
        };

        _context.Projects.Add( project );

        await _context.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}










