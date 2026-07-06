using System;
using System.Collections.Generic;
using System.Text;
using BasicJira.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicJira.Application.Projects.GetProjects;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
// IRequestHandler arayüzü uygulanıyor. uygulamanın bir yerinde GetProjectsQuery gönderildiğinde, MediatR arayıcılığıyla bu
// cevabın kimin vereceğini sistemin bilmesini sağlar. 

{
    private readonly IAppDbContext _context;

    public GetProjectsQueryHandler(IAppDbContext context)
    {
        _context = context;     // sistemin veritabanıyla konuşabilmesi için IAppDbContext constructor içeriye alınır. Dependecy Injection

    }

    public async Task<List<ProjectDto>> Handle(    // request geldiğinde mediatR bu metodu otomatik planda çalıştırır.
        GetProjectsQuery request,
        CancellationToken cancellationToken )

    {

        // AsNoTracking -> Sadece okuma yaptığımız için EF Core'un change tracking yapmasına gerek yok.
        // Bu şekilde query performansı artar. 

        return await _context.Projects
            .AsNoTracking()
            .Select(project => new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,                // veritabanından tüm objeleri değil, sadece DTO'da tanımlı olan alanları alıyoruz.
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            })
            .ToListAsync(cancellationToken);       // üstte hazırlanan sorguları asenkron çalıştırır. sonuçları list döndürür. 
    }
}

/* peki neden db işlemimiz async ?
eğer işlem "ToList" senkron olsaydı o an bu web isteğini işleyen thread veritabanından cevap gelene kadar kilitli kalıp 
hiçbir şey yapmadan beklerdi. Thread Starvation

async ve await kullandığımızda sorgu veritabanına iletilir ve web isteğini tutan mevcut thread anında serbest bırakılır.
serbest kalan bu thread , thread poola döner. diğer kullanıcıların web isteklerini işlemek için kullanılabilir.
veritabanından dönüş geldiğinde, havuzdaki boş bir thread tarafından kaldığı yerden devralınarak tamamlanır.
yani bu sayede elimizdeki kaynakları verimli kullanmış olup performansı senkron işlemlere kıyasla ciddi seviyede arttırmış oluruz.

ayrıca handle metodunu bir cancellationtoken alıyor, istek kesildiğinde asenkron yapı sayesinde istek veritabanı seviyesinde de iptal edilebilir. 
*/