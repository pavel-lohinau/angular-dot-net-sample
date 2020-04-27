using System;
using WebApi.Core.Entities;

namespace WebApi.Core.Interfaces
{
  public interface ICustomerRepository : IAsyncRepository<Customer, Guid>
  {

  }
}
