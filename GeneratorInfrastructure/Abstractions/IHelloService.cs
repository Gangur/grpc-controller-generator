using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Abstractions
{
    public interface IHelloService
    {
        Task<string> HelloAsync(string firstName, string lastName, CancellationToken cancellationToken);
    }
}
