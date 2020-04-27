using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Newtonsoft.Json;
using WebApi.Infrastructure.Identity;
using WebApi.Web;
using WebApi.Web.Models;
using Xunit;

namespace WebApi.Tests.Api
{
  public class AccountTests : IClassFixture<CustomWebApplicationFactory<Startup>>
  {
    private readonly HttpClient _client;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountTests(CustomWebApplicationFactory<Startup> factory)
    {
      _client = factory.CreateClient();
      _userManager = PopulateTestIdentityAsync();
    }

    public static UserManager<ApplicationUser> PopulateTestIdentityAsync()
    {
      _ = Guid.TryParse("1A99B72D-EC37-4BEC-97CD-9767380B667B", out Guid userId);

      var user = new ApplicationUser
      {
        Id = userId.ToString(),
        LastName = "UserLastName",
        FirstName = "UserFirstName",
        Email = "UserEmail@gmail.com",
        UserName = "UserUserName"
      };

      return MockUserManager<ApplicationUser>(new List<ApplicationUser> { user }).Object;
    }

    public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
    {
      var store = new Mock<IUserStore<TUser>>();
      var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
      mgr.Object.UserValidators.Add(new UserValidator<TUser>());
      mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

      mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
      mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.Run(() => { return ls[0]; }));
      
      return mgr;
    }

    [Theory]
    [MemberData(nameof(AccountTests.UserGeneratorForRegisterTest))]
    public async Task CanRegisterAsync(RegisterModel registerModel)
    {
      var stringDTO = JsonConvert.SerializeObject(registerModel);
      using var formContent = new StringContent(stringDTO, Encoding.UTF8, "application/json");
      // Arrange
      var httpResponse = await _client.PostAsync("/api/account/register", formContent).ConfigureAwait(false);

      if (httpResponse.IsSuccessStatusCode == false)
      {
        Assert.True(httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest);
      }
      else
      {
        Assert.True(httpResponse.IsSuccessStatusCode);
      }
    }

    public static IEnumerable<object[]> UserGeneratorForRegisterTest()
    {
      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", FirstName = "UserFirstName", Email = "UserEmail@gmail.com", UserName = "UserUserName", Password = "1234Qwer***" }
      };

      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", FirstName = "UserFirstName", Email = "UserEmail", UserName = "UserUserName", Password = "1234Qwer***" }
      };

      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", FirstName = "UserFirstName", Email = "UserEmail@gmail.com", UserName = "UserUserName", Password = "1234" }
      };

      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", FirstName = "UserFirstName", Email = "UserEmail@gmail.com", UserName = "UserUserName", Password = "1234Qwer***" }
      };

      yield return new object[]
      {
        new RegisterModel { FirstName = "UserFirstName", Email = "UserEmail@gmail.com", UserName = "UserUserName", Password = "1234Qwer***" }
      };

      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", Email = "UserEmail@gmail.com", UserName = "UserUserName", Password = "1234Qwer***" }
      };

      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", FirstName = "UserFirstName", UserName = "UserUserName", Password = "1234Qwer***" }
      };

      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", FirstName = "UserFirstName", Email = "UserEmail@gmail.com", Password = "1234Qwer***" }
      };

      yield return new object[]
      {
        new RegisterModel { LastName = "UserLastName", FirstName = "UserFirstName", Email = "UserEmail@gmail.com", UserName = "UserUserName" }
      };
    }

    [Theory]
    [MemberData(nameof(AccountTests.UserGeneratorForLoginTest))]
    public async Task CanLoginAsync(LoginModel registerModel)
    {
      var stringDTO = JsonConvert.SerializeObject(registerModel);
      using var formContent = new StringContent(stringDTO, Encoding.UTF8, "application/json");
      // Arrange
      var httpResponse = await _client.PostAsync("/api/account/login", formContent).ConfigureAwait(false);

      if (httpResponse.IsSuccessStatusCode == false)
      {
        Assert.True(httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest);
      }
      else
      {
        Assert.True(httpResponse.IsSuccessStatusCode);
       // Assert.False(string.IsNullOrWhiteSpace(result));
      }
    }

    public static IEnumerable<object[]> UserGeneratorForLoginTest()
    {
      yield return new object[]
      {
        new LoginModel { UserName = "UserUserName", Password = "1234Qwer***" }
      };
    }
  }
}
