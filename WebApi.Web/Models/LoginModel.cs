using System.ComponentModel.DataAnnotations;

namespace WebApi.Web.Models
{
  public class LoginModel
  {
    [Required(AllowEmptyStrings = false)]
    [StringLength(maximumLength: 20, MinimumLength = 5)]
    public string UserName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool IsRememberMe { get; set; }
  }
}
