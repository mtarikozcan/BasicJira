using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;


namespace BasicJira.Application.Tasks.Commands.CreateTask;

public class  CreateTaskCommandValidator
    : AbstractValidator<CreateTaskCommand>

{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Priority)
            .IsInEnum();
            
    }

}
