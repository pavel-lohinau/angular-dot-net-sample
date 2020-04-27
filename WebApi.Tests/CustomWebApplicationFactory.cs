using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.Infrastructure.Data;
using WebApi.Infrastructure.Identity;
using WebApi.Web;

namespace WebApi.Tests
{
  public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureServices(async services =>
      {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        services.AddDbContext<AppDbContext>(options =>
        {
          options.UseInMemoryDatabase("InMemoryStoreDb");
          options.UseInternalServiceProvider(serviceProvider);
        });

        services.AddDbContext<IdentityDbContext>(options =>
        {
          options.UseInMemoryDatabase("InMemoryIdentityDb");
          options.UseInternalServiceProvider(serviceProvider);
        });

        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var appDb = scopedServices.GetRequiredService<AppDbContext>();

        var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

        appDb.Database.EnsureCreated();

        try
        {
          await SeedData.PopulateTestDataAsync(appDb).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
          logger.LogError(ex, "An error occurred seeding the " +
                              "database with test messages. Error: {ex.Message}");
        }
      });
    }
  }
}
