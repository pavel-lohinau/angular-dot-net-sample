using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Core.Entities;

namespace WebApi.Core.Interfaces
{
  public interface IAsyncCustomerService
  {
    Task<IReadOnlyCollection<Customer>> GetCustomersAsync();

    Task<Customer> GetCustomerAsync(Guid id);

    Task PostCustomerAsync(Customer customer);

    Task PutCustomerAsync(Guid id, Customer customer);

    Task DeleteCustomerAsync(Guid id, Customer customer);
  }
}