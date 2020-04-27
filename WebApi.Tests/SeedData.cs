using System;
using System.Threading.Tasks;
using WebApi.Core.Entities;
using WebApi.Infrastructure.Data;

namespace WebApi.Tests
{
  class SeedData
  {
    public async static Task PopulateTestDataAsync(AppDbContext dbContext)
    {
      _ = Guid.TryParse("1A99B72D-EC37-4BEC-97CD-9767380B667B", out Guid customerId);

      var customer = new Customer
      {
        Id = customerId,
        LastName = "CustomerLastName",
        FirstName = "CustomerFirstName",
        Address = "CustomerFirstName",
        City = "CustomerCity",
        Email = "CustomerEmail@gmail.com",
        State = "CustomerState",
        Phone = "+375 29 888-77-99",
        Zipcode = "220022"
      };

      await dbContext.Customer.AddAsync(customer);

      await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
  }
}