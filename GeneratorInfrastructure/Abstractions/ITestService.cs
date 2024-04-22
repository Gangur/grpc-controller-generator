using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Abstractions
{
    public interface ITestService
    {
         Task<string> TestAsync(string arg1, CancellationToken cancellationToken);
    }
}
