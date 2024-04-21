using Infrastructure.Abstractions;

namespace Server.Services
{
    public class HelloServiceImpl : IHelloService
    {
        public Task<string> HelloAsync(string firstName, string lastName, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Hello {firstName} {lastName}");
        }
    }
}
