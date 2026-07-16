using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BasicJira.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator
    : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
