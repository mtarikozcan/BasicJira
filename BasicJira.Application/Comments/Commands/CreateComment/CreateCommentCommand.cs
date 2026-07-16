using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Comments.Commands.CreateComment;

public record CreateCommentCommand(
    Guid TaskItemId,
    Guid UserId,
    string Comment
) : IRequest<Guid>;
