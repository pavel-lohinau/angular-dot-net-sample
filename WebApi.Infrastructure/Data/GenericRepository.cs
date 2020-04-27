using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using WebApi.Core.Entities;
using WebApi.Core.Interfaces;

namespace WebApi.Infrastructure.Data
{
  public class GenericRepository<TEntity, TKey> : IAsyncRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
  {
    private readonly AppDbContext _appDbContext;
    private readonly IDistributedCache _distributedCache;
    private const double CacheExpirationInMinutes = 2;

    private DbSet<TEntity> DbSet => _appDbContext.Set<TEntity>();

    public GenericRepository(
      AppDbContext appDbContext,
      IDistributedCache distributedCache)
    {
      _appDbContext = appDbContext;
      _distributedCache = distributedCache;
    }

    public virtual async Task CreateAsync(TKey id, TEntity entity)
    {
      await DbSet.AddAsync(entity);
    }

    public virtual async Task DeleteAsync(TKey id, TEntity entity)
    {
      var cachedItem = await _distributedCache.GetStringAsync(id.ToString()).ConfigureAwait(false);

      if (cachedItem != null)
      {
        await _distributedCache.RemoveAsync(id.ToString()).ConfigureAwait(false);
      }

      DbSet.Remove(entity);
    }

    public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
    {
      var items = await DbSet.ToListAsync().ConfigureAwait(false);

      return items;
    }

    public virtual async Task<TEntity> GetAsync(TKey id)
    {
      var cachedItem = await _distributedCache.GetStringAsync(id.ToString()).ConfigureAwait(false);

      if (cachedItem != null)
      {
        return JsonConvert.DeserializeObject<TEntity>(cachedItem);
      }
      else
      {
        var item = await DbSet.FindAsync(id);

        if (item != null)
        {
          await _distributedCache.SetStringAsync(
            id.ToString(),
            JsonConvert.SerializeObject(item),
            new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(GenericRepository<TEntity, TKey>.CacheExpirationInMinutes))).ConfigureAwait(false);
        }

        return item;
      }
    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
      var item = await DbSet.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);

      return item;
    }

    public virtual async Task UpdateAsync(TKey id, TEntity entity)
    {
      var cachedItem = await _distributedCache.GetStringAsync(id.ToString()).ConfigureAwait(false);

      if (cachedItem != null)
      {
        await _distributedCache.RemoveAsync(id.ToString()).ConfigureAwait(false);
      }

      var entityState = DbSet.Update(entity);

      if (entityState.State == EntityState.Modified)
      {
        await _distributedCache.SetStringAsync(
          id.ToString(),
          JsonConvert.SerializeObject(entity),
          new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(GenericRepository<TEntity, TKey>.CacheExpirationInMinutes))).ConfigureAwait(false);
      }
    }
  }
}