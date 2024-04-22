using Grpc.Core;
namespace Server.Controllers
{
    internal partial class TestController 
    {
        private readonly Infrastructure.Abstractions.ITestService _itestservice;
        
        public TestController(Infrastructure.Abstractions.ITestService itestservice)
        {
            _itestservice = itestservice;
        }
        
        public async override partial Task<TestResponse> Testing(
            TestRequest request,
            ServerCallContext context)
        {
            var result = await _itestservice.TestAsync(
                request.Arg1,
                context.CancellationToken);
        
            return new TestResponse()
            {
                Message = result
            };
        }
            
    }
};