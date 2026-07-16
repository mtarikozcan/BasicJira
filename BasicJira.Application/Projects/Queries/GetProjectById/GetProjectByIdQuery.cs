using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.DTOs;
using MediatR;

namespace BasicJira.Application.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;
