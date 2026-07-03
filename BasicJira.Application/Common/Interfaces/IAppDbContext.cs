using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Common.Interfaces;

public interface IAppDbContext
{ 
    // these DbSets represent the tables that Application handlers need to access.
    DbSet<Project> Projects { get; }

    DbSet<AppUser> Users { get; }

    DbSet<TaskItem> TaskItems { get; }

    DbSet<TaskComment> TaskComments { get; }

    // handlers call this after Add/Update/Delete operations.

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
