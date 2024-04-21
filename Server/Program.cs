using Server.Controllers;
using Server.Services;
using G = Grpc.Core;

public partial class Program
{
    const int _port = 7777;

    static async Task Main(string[] args)
    {
        G.Server server = new()
        {
            Ports = { new G.ServerPort("localhost", _port, G.ServerCredentials.Insecure) },
            Services = { HelloService.BindService(new HelloController(new HelloServiceImpl())) }
        };

        try
        {
            server.Start();
            Console.WriteLine($"Server is listening to port {_port}");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error has been thrown: " + ex);
        }
        finally
        {
            if (server is not null)
            {
                await server.ShutdownAsync();
            }
        }
    }
}