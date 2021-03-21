using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Grpc client started ...");

            var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            using var channel = GrpcChannel.ForAddress("https://localhost:5001",
                new GrpcChannelOptions { LoggerFactory = loggerFactory });

            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
              new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
