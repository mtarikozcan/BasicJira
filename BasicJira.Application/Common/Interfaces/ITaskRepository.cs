using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Entities;

namespace BasicJira.Application.Common.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<List<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);
}