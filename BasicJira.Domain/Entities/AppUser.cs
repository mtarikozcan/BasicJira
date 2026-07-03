using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
     
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();


}
