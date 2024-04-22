using Grpc.Core;
using Infrastructure.Abstractions;
using Infrastructure.Attributes;
using static TestService;

namespace Server.Controllers
{
    [GrpcMapping]
    internal class TestController : TestServiceBase
    {
        [GrpcInterface(typeof(ITestService), nameof(ITestService.TestAsync))]
        public override partial Task<TestResponse> Testing(
           TestRequest request,
           ServerCallContext context);
    }
}
