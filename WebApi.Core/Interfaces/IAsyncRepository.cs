using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Core.Interfaces
{
  public interface IAsyncRepository<TEntity, in TKey> where TEntity : class
  {
    Task<IReadOnlyCollection<TEntity>> GetAllAsync();

    Task<TEntity> GetAsync(TKey id);

    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task CreateAsync(TKey id, TEntity entity);

    Task UpdateAsync(TKey id, TEntity entity);

    Task DeleteAsync(TKey id, TEntity entity);
  }
}