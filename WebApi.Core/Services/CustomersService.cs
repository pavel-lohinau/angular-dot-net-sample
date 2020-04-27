using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Core.Entities;
using WebApi.Core.Interfaces;

namespace WebApi.Core.Services
{
  public class CustomersService : IAsyncCustomerService
  {
    private readonly IUnitOfWork _unitOfWork;

    public CustomersService(
      IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<Customer>> GetCustomersAsync()
    {
      var customers = await _unitOfWork.CustomerRepository.GetAllAsync().ConfigureAwait(false);

      return customers;
    }

    public async Task<Customer> GetCustomerAsync(Guid id)
    {
      var customer = await _unitOfWork.CustomerRepository.GetAsync(id).ConfigureAwait(false);

      return customer;
    }

    public async Task PostCustomerAsync(Customer customer)
    {
      await _unitOfWork.CustomerRepository.CreateAsync(customer.Id, customer).ConfigureAwait(false);

      await _unitOfWork.CommitAsync().ConfigureAwait(false);
    }

    public async Task PutCustomerAsync(Guid id, Customer customer)
    {
      await _unitOfWork.CustomerRepository.UpdateAsync(id, customer).ConfigureAwait(false);

      await _unitOfWork.CommitAsync().ConfigureAwait(false);
    }
    
    public async Task DeleteCustomerAsync(Guid id, Customer customer)
    {
      await _unitOfWork.CustomerRepository.DeleteAsync(id, customer).ConfigureAwait(false);

      await _unitOfWork.CommitAsync().ConfigureAwait(false);
    }
  }
}