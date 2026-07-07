using System;
using System.Collections.Generic;
using System.Text;

using MediatR;
using BasicJira.Application.DTOs; 

namespace BasicJira.Application.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto>; 