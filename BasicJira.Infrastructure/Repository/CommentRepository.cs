using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;

namespace BasicJira.Infrastructure.Repositories;

public class CommentRepository : Repository<TaskComment>, ICommentRepository
{
    public CommentRepository(IAppDbContext context)
        : base(context)
    {
    }
}
