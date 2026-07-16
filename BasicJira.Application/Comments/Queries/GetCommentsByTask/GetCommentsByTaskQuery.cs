using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.DTOs;
using MediatR;

namespace BasicJira.Application.Comments.Queries.GetCommentsByTask;

public record GetCommentsByTaskQuery(Guid TaskItemId) : IRequest<List<CommentDto>>;
