using Grpc.Core;
namespace Server.Controllers
{
    internal partial class HelloController 
    {
        private readonly Infrastructure.Abstractions.IHelloService _ihelloservice;
        
        public HelloController(Infrastructure.Abstractions.IHelloService ihelloservice)
        {
            _ihelloservice = ihelloservice;
        }
        
        public async override partial Task<HelloResponse> Welcome(
            HelloRequest request,
            ServerCallContext context)
        {
            var result = await _ihelloservice.HelloAsync(
                request.FirstName, request.LastName,
                context.CancellationToken);
        
            return new HelloResponse()
            {
                Message = result
            };
        }
        public async override partial Task<HelloResponse> Welcome2(
            HelloRequest request,
            ServerCallContext context)
        {
            var result = await _ihelloservice.HelloAsync(
                request.FirstName, request.LastName,
                context.CancellationToken);
        
            return new HelloResponse()
            {
                Message = result
            };
        }
            
    }
};