using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Web.Models
{
  public class CustomerDTO
  {
    public CustomerDTO()
    {
      //Order = new HashSet<OrderDTO>();
    }

    public Guid CustomerId { get; set; }

    [StringLength(50)]
    [Required]
    public string FirstName { get; set; }

    [StringLength(50)]
    [Required]
    public string LastName { get; set; }

    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Phone]
    [Required]
    public string Phone { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Zipcode { get; set; }

    //public virtual ICollection<OrderDTO> Order { get; }
  }
}
