using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Infrastructure.Identity
{
  [Table("AspNetUsers")]
  public class ApplicationUser : IdentityUser
  {
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string RefreshToken { get; set; }
  }
}
