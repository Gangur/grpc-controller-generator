using Grpc.Core;
using Server.Notifications;

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

       public async override partial Task StreamWelcome(
            HelloRequest request, 
            IServerStreamWriter<HelloResponse> response, 
            ServerCallContext context)
        {
            await foreach(var notification in _ihelloservice.HelloStream(
                    request.FirstName, request.LastName,
                    context.CancellationToken))
                    {
                        switch(notification)
                        {
                            case HelloNotification hellonotification:
                                await response.WriteAsync(new Server.Messages.HelloMessage(
                                    hellonotification.FirstName, hellonotification.LastName
                                ));
                            break;
                            case WelcomeNotification welcomenotification:
                                await response.WriteAsync(new Server.Messages.WelcomeMessage(
                                    welcomenotification.FullName, welcomenotification.Age
                                ));
                            break;
                            default:
                                throw new InvalidCastException($"Unexpected notification type {notification.GetType().FullName}");
                        }
                    }
        }

    }
};