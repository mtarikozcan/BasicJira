using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IAppDbContext _context;

    public GetUsersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
