using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure.Identity
{
  public class IdentityDbContext : IdentityDbContext<ApplicationUser>
  {
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) {
      Database.Migrate();
    }
  }
}
