using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
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
            //var reply = await client.SayHelloAsync(
            //  new HelloRequest { Name = "GreeterClient" });
            //Console.WriteLine("Greeting: " + reply.Message);

            //var token = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            //using var replyStreamServer = client.StreamingServer(
            //    new HelloRequest { Name = "StreamingServer" },
            //    cancellationToken: token.Token);
            //try
            //{
            //    await foreach (var item in replyStreamServer.ResponseStream.ReadAllAsync(token.Token))
            //    { 
            //        Console.WriteLine(item.Message);
            //    }
            //}
            //catch (RpcException exc)
            //{
            //    Console.WriteLine(exc.Message);
            //}

            using var replyStreamClient = client.StreamingClient();

            foreach (var name in new[] { "Stacking", "Client", "Stream" })
            {
                await replyStreamClient.RequestStream.WriteAsync(new HelloRequest
                {
                    Name = name
                });
            }

            await replyStreamClient.RequestStream.CompleteAsync();
            var response = await replyStreamClient.ResponseAsync;

            Console.WriteLine(response.Count);
            Console.WriteLine(response.Message);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
