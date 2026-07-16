using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Tasks.Commands.ChangeTaskStatus;

public class ChangeTaskStatusCommandValidator
    : AbstractValidator<ChangeTaskStatusCommand>
{
    public ChangeTaskStatusCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
