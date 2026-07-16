using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Comments.Commands.DeleteComment;

public class DeleteCommentCommandValidator
    : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
