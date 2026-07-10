using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Entities;

namespace BasicJira.Application.Common.Interfaces;

public interface IUserRepository : IRepository<AppUser>
{
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken);
}