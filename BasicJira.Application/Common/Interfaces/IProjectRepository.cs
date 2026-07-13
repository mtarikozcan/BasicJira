using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.DTOs;
using BasicJira.Domain.Entities;

namespace BasicJira.Application.Common.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<bool> ExistsAsync(Guid projectId, CancellationToken cancellationToken);

    Task<List<ProjectDto>> GetAllAsync(CancellationToken cancellationToken);
}

// jenerik + entity özgü sınıflar. ortak crud methodları inherit aldı 