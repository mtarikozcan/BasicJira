using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Tasks.Commands.UnassignUserFromTask;

public record UnassignUserFromTaskCommand(Guid TaskId) : IRequest;
