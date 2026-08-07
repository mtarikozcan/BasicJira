using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Projects.Commands.RemoveProjectMember;

public sealed class RemoveProjectMemberCommandHandler
    : IRequestHandler<RemoveProjectMemberCommand>
{
    private readonly IAppDbContext _dbContext;

    public RemoveProjectMemberCommandHandler(
        IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(
        RemoveProjectMemberCommand request,
        CancellationToken cancellationToken)
    {
        var membership = await _dbContext.ProjectMembers
            .FirstOrDefaultAsync(
                x => x.ProjectId == request.ProjectId &&
                     x.UserId == request.UserId,
                cancellationToken);

        if (membership is null)
            throw new NotFoundException(
                "ProjectMember",
                $"{request.ProjectId}:{request.UserId}");

        _dbContext.ProjectMembers.Remove(membership);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}