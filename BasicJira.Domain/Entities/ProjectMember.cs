namespace BasicJira.Domain.Entities;

public sealed class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}