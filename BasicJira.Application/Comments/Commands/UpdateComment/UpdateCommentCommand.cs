using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(
    Guid Id,
    string Comment
) : IRequest;
