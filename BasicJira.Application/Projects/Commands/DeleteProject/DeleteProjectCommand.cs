using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest;
