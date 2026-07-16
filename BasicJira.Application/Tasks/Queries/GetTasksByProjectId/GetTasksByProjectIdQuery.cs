using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.DTOs;
using MediatR;

namespace BasicJira.Application.Tasks.Queries.GetTasksByProjectId;

public record GetTasksByProjectIdQuery(Guid ProjectId) : IRequest<List<TaskDto>>;
