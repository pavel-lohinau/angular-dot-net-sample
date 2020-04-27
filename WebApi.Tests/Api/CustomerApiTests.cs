using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApi.Infrastructure.Identity;
using WebApi.Web;
using WebApi.Web.Helpers;
using WebApi.Web.Models;
using Xunit;

namespace WebApi.Tests.Api
{
  public class CustomerApiTests : IClassFixture<CustomWebApplicationFactory<Startup>>
  {
    private readonly HttpClient _client;

    public CustomerApiTests(CustomWebApplicationFactory<Startup> factory)
    {
      _client = factory.CreateClient();
      _client.DefaultRequestHeaders.Authorization = 
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CustomerApiTests.GenerateJwtToken());
    }

    public static string GenerateJwtToken()
    {
      var expiresDate = DateTime.Now.AddHours(2);

      var token = new JwtSecurityToken
      (
          issuer: "store.io",
          audience: "audience",
          expires: expiresDate,
          notBefore: DateTime.Now,
          signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes("6c54a215b7f3f14b608a72468a2ef9bf03934478ac5aa37de40f3c9a8293575b")),
                  SecurityAlgorithms.HmacSha256)
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task CanGetCustomersAsync()
    {
      // Arrange
      var httpResponse = await _client.GetAsync("/api/customers").ConfigureAwait(false);
      httpResponse.EnsureSuccessStatusCode();

      // Act
      var stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
      var customers = JsonConvert.DeserializeObject<IEnumerable<CustomerDTO>>(stringResponse);

      // Assert
      Assert.Contains(customers, p => p.FirstName == "CustomerFirstName");
    }

    [Theory]
    [InlineData("1A99B72D-EC37-4BEC-97CD-9767380B667B")]
    [InlineData("57C87984-D9F9-49D2-9A12-92B444C3FF99")]
    public async Task CanGetCustomerAsync(Guid id)
    {
      // Arrange
      var httpResponse = await _client.GetAsync($"/api/customers/{id}").ConfigureAwait(false);

      if (httpResponse.IsSuccessStatusCode)
      {
        // Act
        var stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        var customer = JsonConvert.DeserializeObject<CustomerDTO>(stringResponse);

        // Assert
        Assert.Equal(customer.CustomerId, id);
      }
      else
      {
        Assert.True(httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound);
      }
    }

    [Theory]
    [MemberData(nameof(CustomerApiTests.CustomerGeneratorForPostTest))]
    public async Task CanPostCustomerAsync(CustomerDTO customerDTO)
    {
      var stringDTO = JsonConvert.SerializeObject(customerDTO);
      using var formContent = new StringContent(stringDTO, Encoding.UTF8, "application/json");

      var httpResponse = await _client.PostAsync("/api/customers", formContent).ConfigureAwait(false);

      if (httpResponse.IsSuccessStatusCode)
      {
        var stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        var customer = JsonConvert.DeserializeObject<CustomerDTO>(stringResponse);

        Assert.Equal(customerDTO.FirstName, customer.FirstName);
      }
      else
      {
        Assert.True(httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest);
      }
    }

    public static IEnumerable<object[]> CustomerGeneratorForPostTest()
    {
      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomerLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };

      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomerLastName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };
      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomerLastName", FirstName = "CustsdfffffffffffffffffffffffffffffffffffffffffffffffffffffomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };
      yield return new object[]
      {
        new CustomerDTO { FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };
      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomeraaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };
      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomerLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };
      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomerLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };
      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomerLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Zipcode = "220022" }
      };
      yield return new object[]
      {
        new CustomerDTO { LastName = "CustomerLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "phone", Zipcode = "220022" }
      };
    }

    [Theory]
    [MemberData(nameof(CustomerApiTests.CustomerGeneratorForPutTest))]
    public async Task CanPutCustomerAsync(Guid id, CustomerDTO customerDTO)
    {
      var stringDTO = JsonConvert.SerializeObject(customerDTO);
      using var formContent = new StringContent(stringDTO, Encoding.UTF8, "application/json");

      var httpResponse = await _client.PutAsync($"/api/customers/{id}", formContent).ConfigureAwait(false);

      if (httpResponse.IsSuccessStatusCode)
      {
        var stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        var customer = JsonConvert.DeserializeObject<CustomerDTO>(stringResponse);

        Assert.Equal(customerDTO.LastName, customer.LastName);
      }
      else
      {
        Assert.True(httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest);
      }
    }

    public static IEnumerable<object[]> CustomerGeneratorForPutTest()
    {
      _ = Guid.TryParse("1A99B72D-EC37-4BEC-97CD-9767380B667B", out Guid customerId);
      _ = Guid.TryParse("57C87984-D9F9-49D2-9A12-92B444C3FF99", out Guid wrongCustomerId);

      yield return new object[]
      {
        customerId, new CustomerDTO { CustomerId = customerId, LastName = "UpdatedLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };

      yield return new object[]
      {
        wrongCustomerId, new CustomerDTO { CustomerId = customerId, LastName = "UpdatedLastName", FirstName = "CustomerFirstName", Address = "CustomerFirstName", City = "CustomerCity", Email = "CustomerEmail@gmail.com", State = "CustomerState", Phone = "+375 29 888-77-99", Zipcode = "220022" }
      };
    }

    [Theory]
    [InlineData("1A99B72D-EC37-4BEC-97CD-9767380B667B")]
    [InlineData("57C87984-D9F9-49D2-9A12-92B444C3FF99")]
    public async Task CanDeleteCustomerAsync(Guid id)
    {
      var httpResponse = await _client.DeleteAsync($"/api/customers/{id}").ConfigureAwait(false);
      if (httpResponse.IsSuccessStatusCode == false)
      {
        Assert.True(httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound);
      }
      else
      {
        Assert.True(httpResponse.IsSuccessStatusCode);
      }
    }
  }
}