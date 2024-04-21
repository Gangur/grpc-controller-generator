using Grpc.Core;

const int _port = 7777;

Channel channel = new Channel($"localhost:{_port}", ChannelCredentials.Insecure);

try
{
    await channel.ConnectAsync();
    Console.WriteLine("The client connected succesfully to the server");

    var client = new HelloService.HelloServiceClient(channel);
    var respose = await client.WelcomeAsync(new HelloRequest
    {
        FirstName = "Dmitrii",
        LastName = "Gangur"
    });

    Console.WriteLine(respose.Message);

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex);
}
finally
{
    if (channel is not null)
    {
        await channel.ShutdownAsync();
    }
}