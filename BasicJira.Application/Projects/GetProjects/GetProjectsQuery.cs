using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BasicJira.Application.Projects.GetProjects;

// query -> veri değiştirmez. sadece veri okur.
//IRequest<List<ProjectDto>>: MediatR kütüphanesinin bir parçasıdır ve bu sınıfın bir "request" olduğunu belirtir.
//Bu request, bir liste döndürecektir ve bu liste ProjectDto tipindedir.

public record GetProjectsQuery : IRequest<List<ProjectDto>>;
