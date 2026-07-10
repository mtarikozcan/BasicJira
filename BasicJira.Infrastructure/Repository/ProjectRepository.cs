using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.DTOs;
using BasicJira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(IAppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await Context.Projects
            .AnyAsync(x => x.Id == projectId, cancellationToken);
    }

    public async Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await Context.Projects
            .AsNoTracking()
            .Select(project => new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            })
            .ToListAsync(cancellationToken);
    }
}
