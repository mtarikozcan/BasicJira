using MediatR;

namespace BasicJira.Application.Projects.Commands.RemoveProjectMember;

public sealed record RemoveProjectMemberCommand(
    Guid ProjectId,
    Guid UserId) : IRequest;