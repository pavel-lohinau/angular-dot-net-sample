using System.ComponentModel.DataAnnotations;

namespace WebApi.Web.Models
{
  public class RegisterModel
  {
    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 20, MinimumLength = 5)]
    public string UserName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 20, MinimumLength = 1)]
    public string FirstName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 20, MinimumLength = 1)]
    public string LastName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string Email { get; set; }
  }
}
