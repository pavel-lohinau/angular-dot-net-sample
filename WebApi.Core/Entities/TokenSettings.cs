using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Core.Entities
{
  public class TokenSettings
  {
    public string Secret { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }

    public double AccessExpiration { get; set; }

    public SymmetricSecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
  }
}
