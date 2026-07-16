using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Domain.Entities;

namespace BasicJira.Application.Common.Interfaces;

public interface ICommentRepository : IRepository<TaskComment>
{
}
