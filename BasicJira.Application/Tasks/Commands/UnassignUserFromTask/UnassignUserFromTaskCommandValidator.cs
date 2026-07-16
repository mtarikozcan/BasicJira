using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Tasks.Commands.UnassignUserFromTask;

public class UnassignUserFromTaskCommandValidator
    : AbstractValidator<UnassignUserFromTaskCommand>
{
    public UnassignUserFromTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty();
    }
}
