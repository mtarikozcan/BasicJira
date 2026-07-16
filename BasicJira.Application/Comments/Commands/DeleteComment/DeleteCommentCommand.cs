using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid Id) : IRequest;
