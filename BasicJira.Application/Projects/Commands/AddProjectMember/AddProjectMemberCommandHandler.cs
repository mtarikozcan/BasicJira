using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Projects.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandHandler
    : IRequestHandler<AddProjectMemberCommand>
{
    private readonly IAppDbContext _dbContext;

    public AddProjectMemberCommandHandler(
        IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(
        AddProjectMemberCommand request,
        CancellationToken cancellationToken)
    {
        var projectExists = await _dbContext.Projects
            .AnyAsync(
                x => x.Id == request.ProjectId,
                cancellationToken);

        if (!projectExists)
            throw new NotFoundException(
                nameof(Project),
                request.ProjectId);

        var userExists = await _dbContext.Users
            .AnyAsync(
                x => x.Id == request.UserId,
                cancellationToken);

        if (!userExists)
            throw new NotFoundException(
                nameof(AppUser),
                request.UserId);

        var membershipExists = await _dbContext.ProjectMembers
            .AnyAsync(
                x => x.ProjectId == request.ProjectId &&
                     x.UserId == request.UserId,
                cancellationToken);

        if (membershipExists)
            throw new ConflictException(
                "User is already a member of this project.");

        _dbContext.ProjectMembers.Add(
            new ProjectMember
            {
                ProjectId = request.ProjectId,
                UserId = request.UserId
            });

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}