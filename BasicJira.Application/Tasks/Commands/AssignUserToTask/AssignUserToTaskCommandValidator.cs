using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Tasks.Commands.AssignUserToTask;

public class AssignUserToTaskCommandValidator
    : AbstractValidator<AssignUserToTaskCommand>
{
    public AssignUserToTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
