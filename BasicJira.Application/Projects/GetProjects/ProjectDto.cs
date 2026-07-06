using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Application.Projects.GetProjects;

public class  ProjectDto
{
    // API response için entitynin tamamını değil, sadece client a göstermek istediğimiz alanları taşırız.
    // örneğin entitiynin direkt response oalrak dönmesi client in passwordh hash gibi secret bilgilerini görmesine sebep olur.

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

}

