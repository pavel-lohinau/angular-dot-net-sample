using System;

namespace WebApi.Core.Entities
{
  public class BaseEntity<T>
  {
    public T Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }
  }
}
