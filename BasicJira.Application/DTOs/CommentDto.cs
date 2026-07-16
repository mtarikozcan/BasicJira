using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Application.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }

    public Guid TaskItemId { get; set; }

    public Guid UserId { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
