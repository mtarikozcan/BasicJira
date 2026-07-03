using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Domain.Entities;

public class TaskComment
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

}

