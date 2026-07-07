using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace BasicJira.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(
        string Name,
        string? Description,
        DateTime StartDate,
        DateTime? EndDate

    ) : IRequest<Guid>;   

    /*
     : IRequest<Guid>; bu command çalışınca oluşan projeye unique id dönecek. Bu idyi kullanarak projeyi bulabiliriz. guuid 
    CreateProjectCommand , MediatR kütüphanesinin IRequest arayüzü, bu commandin bir request olduğunu belirtir. 
    IRequest arayüzü, MediatR kütüphanesi tarafından kullanılan bir işaretleyici arayüzdür ve bu commandin bir response döndüreceğini belirtir
     */

