using Microsoft.EntityFrameworkCore;
using WebApi.Core.Entities;

namespace WebApi.Infrastructure.Data
{
  public partial class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
      Database.Migrate();
    }

    public virtual DbSet<Customer> Customer { get; set; }
  }
}
