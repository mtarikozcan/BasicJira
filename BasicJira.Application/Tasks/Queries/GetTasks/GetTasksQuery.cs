using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.DTOs;
using MediatR;

namespace BasicJira.Application.Tasks.Queries.GetTasks;

public record GetTasksQuery : IRequest<List<TaskDto>>;
