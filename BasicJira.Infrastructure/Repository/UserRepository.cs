using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Infrastructure.Repositories;

public class UserRepository : Repository<AppUser>, IUserRepository
{
    public UserRepository(IAppDbContext context)
        : base(context)
    {
    }

    public async Task<AppUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .FirstOrDefaultAsync(
                user => user.Email == email,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await Context.Users
            .AnyAsync(
                user => user.Id == userId,
                cancellationToken);
    }
}