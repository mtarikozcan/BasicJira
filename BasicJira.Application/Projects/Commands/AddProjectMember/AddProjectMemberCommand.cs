using MediatR;

namespace BasicJira.Application.Projects.Commands.AddProjectMember;

public sealed record AddProjectMemberCommand(
    Guid ProjectId,
    Guid UserId) : IRequest;