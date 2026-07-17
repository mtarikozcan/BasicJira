using BasicJira.Domain.Entities;

namespace BasicJira.Application.Common.Interfaces;

public interface IUserRepository : IRepository<AppUser>
{
    Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<AppUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}