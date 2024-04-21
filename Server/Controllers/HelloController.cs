using Grpc.Core;
using Infrastructure.Abstractions;
using Infrastructure.Attributes;
using static HelloService;

namespace Server.Controllers
{
    [GrpcMapping]
    internal partial class HelloController : HelloServiceBase
    {
        [GrpcInterface(typeof(IHelloService), nameof(IHelloService.HelloAsync))]
        public override partial Task<HelloResponse> Welcome(
           HelloRequest request,
           ServerCallContext context);

        [GrpcInterface(typeof(IHelloService), nameof(IHelloService.HelloAsync))]
        public override partial Task<HelloResponse> Welcome2(
           HelloRequest request,
           ServerCallContext context);
    }
}