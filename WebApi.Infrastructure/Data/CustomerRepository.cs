using System;
using Microsoft.Extensions.Caching.Distributed;
using WebApi.Core.Entities;
using WebApi.Core.Interfaces;

namespace WebApi.Infrastructure.Data
{
  public class CustomerRepository : GenericRepository<Customer, Guid>, ICustomerRepository
  {
    private readonly AppDbContext _appDbContext;
    private readonly IDistributedCache _distributedCache;

    public CustomerRepository(AppDbContext appDbContext,
      IDistributedCache distributedCache) : base(appDbContext, distributedCache)
    {
      _appDbContext = appDbContext;
      _distributedCache = distributedCache;
    }
  }
}
