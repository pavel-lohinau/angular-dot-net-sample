using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Core.Entities;
using WebApi.Core.Interfaces;
using WebApi.Web.Models;

namespace WebApi.Web.Api
{
  public class CustomersController : BaseApiController
  {
    private readonly IAsyncCustomerService _customerService;
    private readonly IMapper _mapper;

    public CustomersController(
      IAsyncCustomerService customerService,
      IMapper mapper)
    {
      _customerService = customerService;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomersAsync()
    {
      var result = await _customerService.GetCustomersAsync().ConfigureAwait(false);

      var customersDTO = _mapper.Map<IReadOnlyCollection<Customer>, IReadOnlyCollection<CustomerDTO>>(result);

      return Ok(customersDTO);
    }

    [HttpGet("{id:guid}"), ActionName(nameof(GetCustomerAsync))]
    public async Task<IActionResult> GetCustomerAsync(Guid id)
    {
      var customer = await _customerService.GetCustomerAsync(id).ConfigureAwait(false);

      if (customer == null)
      {
        return NotFound();
      }

      var customerDTO = _mapper.Map<CustomerDTO>(customer);

      return Ok(customerDTO);
    }

    [HttpPost]
    public async Task<IActionResult> PostCustomerAsync([FromBody] CustomerDTO customerDTO)
    {
      var customer = _mapper.Map<Customer>(customerDTO);

      await _customerService.PostCustomerAsync(customer).ConfigureAwait(false);

      return CreatedAtAction(nameof(this.GetCustomerAsync), new { id = customer.Id }, customer);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutCustomerAsync(Guid id, CustomerDTO customerDTO)
    {
      if (id != customerDTO.CustomerId)
      {
        return BadRequest(ModelState);
      }

      var customer = _mapper.Map<Customer>(customerDTO);

      await _customerService.PutCustomerAsync(id, customer).ConfigureAwait(false);

      return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCustomerAsync(Guid id)
    {
      var result = await _customerService.GetCustomerAsync(id).ConfigureAwait(false);

      if (result == null)
      {
        return NotFound();
      }

      await _customerService.DeleteCustomerAsync(id, result).ConfigureAwait(false);

      return NoContent();
    }
  }
}