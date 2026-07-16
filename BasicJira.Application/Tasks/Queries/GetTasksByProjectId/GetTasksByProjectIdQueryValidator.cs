using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Tasks.Queries.GetTasksByProjectId;

public class GetTasksByProjectIdQueryValidator
    : AbstractValidator<GetTasksByProjectIdQuery>
{
    public GetTasksByProjectIdQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
