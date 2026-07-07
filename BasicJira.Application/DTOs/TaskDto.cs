using BasicJira.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

using System;

namespace BasicJira.Application.DTOs;

public class TaskDto
{
   
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? AssignedUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskItemStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}