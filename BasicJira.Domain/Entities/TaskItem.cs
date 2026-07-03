using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Enums;

namespace BasicJira.Domain.Entities;
    public class TaskItem 
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid AssignedUserId { get; set; }
    public AppUser AssignedUser { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();

}
