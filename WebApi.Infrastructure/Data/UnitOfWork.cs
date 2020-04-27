using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApi.Core.Entities;
using WebApi.Core.Interfaces;

namespace WebApi.Infrastructure.Data
{
  public class UnitOfWork : IDisposable, IUnitOfWork
  {
    private readonly AppDbContext _appDbContext;
    private readonly IDistributedCache _distributedCache;

    private bool disposed = false;

    public ICustomerRepository CustomerRepository => new CustomerRepository(_appDbContext, _distributedCache);

    public UnitOfWork(AppDbContext appDbContext,
      IDistributedCache distributedCache)
    {
      _appDbContext = appDbContext;
      _distributedCache = distributedCache;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          _appDbContext.Dispose();
        }

        disposed = true;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~UnitOfWork()
    {
      this.Dispose(false);
    }

    public Task CommitAsync()
    {
      this.AddAuitInfo();
      return _appDbContext.SaveChangesAsync();
    }

    private void AddAuitInfo()
    {
      var entries = _appDbContext.ChangeTracker.Entries().Where(x => x.Entity is BaseEntity<Guid> && (x.State == EntityState.Added || x.State == EntityState.Modified));

      foreach (var entry in entries)
      {
        if (entry.State == EntityState.Added)
        {
          ((BaseEntity<Guid>)entry.Entity).Created = DateTime.Now;
        }
          ((BaseEntity<Guid>)entry.Entity).Modified = DateTime.Now;
      }
    }

    public void RejectChanges()
    {
      throw new NotImplementedException();
    }
  }
}
