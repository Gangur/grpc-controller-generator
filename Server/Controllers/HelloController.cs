using Grpc.Core;
using Infrastructure.Abstractions;
using Infrastructure.Attributes;
using Server.Messages;
using Server.Notifications;
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

        [GrpcInterface(typeof(IHelloService), nameof(IHelloService.HelloStream))]
        [GrpcStreamMapping(typeof(HelloNotification), typeof(HelloMessage))]
        [GrpcStreamMapping(typeof(WelcomeNotification), typeof(WelcomeMessage))]
        public override partial Task StreamWelcome(
            HelloRequest request, 
            IServerStreamWriter<HelloResponse> response, 
            ServerCallContext context);
    }
}