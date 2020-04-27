using System.Threading.Tasks;

namespace WebApi.Core.Interfaces
{
  public interface IUnitOfWork
  {
    ICustomerRepository CustomerRepository { get; }

    Task CommitAsync();

    void RejectChanges();

    void Dispose();
  }
}
