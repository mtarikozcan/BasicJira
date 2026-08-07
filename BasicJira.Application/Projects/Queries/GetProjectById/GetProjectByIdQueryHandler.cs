using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.DTOs;
using BasicJira.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BasicJira.Application.Common.Authorization;

namespace BasicJira.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetProjectByIdQueryHandler(
        IAppDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        if (_currentUserService.Role != Roles.Admin)
        {
            var currentUserId = _currentUserService.UserId
                ?? throw new ForbiddenException();

            var isMember = await _context.ProjectMembers
                .AsNoTracking()
                .AnyAsync(
                    member =>
                        member.ProjectId == request.Id &&
                        member.UserId == currentUserId,
                    cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException("You are not a member of this project.");
            }
        }

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };
    }
}
