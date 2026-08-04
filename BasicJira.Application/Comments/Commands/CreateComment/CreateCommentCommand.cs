using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Comments.Commands.CreateComment;

public sealed record CreateCommentCommand(
    Guid TaskItemId,
    string Comment) : IRequest<Guid>;
