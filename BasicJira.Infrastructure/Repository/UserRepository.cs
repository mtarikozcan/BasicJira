using System;
using System.Collections.Generic;
using System.Text;

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

    public async Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await Context.Users
            .AnyAsync(x => x.Id == userId, cancellationToken);
    }
}