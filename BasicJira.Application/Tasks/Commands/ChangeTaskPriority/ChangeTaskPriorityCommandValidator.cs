using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Tasks.Commands.ChangeTaskPriority;

public class ChangeTaskPriorityCommandValidator
    : AbstractValidator<ChangeTaskPriorityCommand>
{
    public ChangeTaskPriorityCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty();

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}
