using BasicJira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Project> Projects { get; }

    DbSet<AppUser> Users { get; }

    DbSet<TaskItem> TaskItems { get; }

    DbSet<TaskComment> TaskComments { get; }

    DbSet<ProjectMember> ProjectMembers { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}