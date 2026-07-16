using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid Id) : IRequest;
