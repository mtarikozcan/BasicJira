using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Comments.Queries.GetCommentsByTask;

public class GetCommentsByTaskQueryValidator
    : AbstractValidator<GetCommentsByTaskQuery>
{
    public GetCommentsByTaskQueryValidator()
    {
        RuleFor(x => x.TaskItemId)
            .NotEmpty();
    }
}
