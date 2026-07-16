using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(
    Guid Id,
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate
) : IRequest;
