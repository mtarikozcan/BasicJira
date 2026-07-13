using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Application.Common.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    void Update(TEntity entity);

    void Remove(TEntity entity);
}

// tüm entityler içerisinde bulunan metodları tanımladık. amac dry engellemek için jenerik interface 