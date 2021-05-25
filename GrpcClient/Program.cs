using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
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

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //var defaultMethodConfig = new MethodConfig
            //{
            //    Names = { MethodName.Default },
            //    RetryPolicy = new RetryPolicy
            //    {
            //        MaxAttempts = 5,
            //        InitialBackoff = TimeSpan.FromSeconds(1),
            //        MaxBackoff = TimeSpan.FromSeconds(5),
            //        BackoffMultiplier = 1.5,
            //        RetryableStatusCodes = { StatusCode.Unavailable }
            //    }
            //};

            //var channel = GrpcChannel.ForAddress("http://localhost:5000",
            //    new GrpcChannelOptions
            //    {
            //        LoggerFactory = loggerFactory,
            //        ServiceConfig = new ServiceConfig
            //        {
            //            MethodConfigs = { defaultMethodConfig }
            //        }
            //    });

            var defaultMethodConfig = new MethodConfig
            {
                Names = { MethodName.Default },
                HedgingPolicy = new HedgingPolicy
                {
                    MaxAttempts = 5,
                    NonFatalStatusCodes = { StatusCode.Unavailable }
                }
            };

            var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
            {
                LoggerFactory = loggerFactory,
                ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }
            });

            //拦截器
            var invoker = channel.Intercept(new ClientLoggerInterceptor());

            var client = new Greeter.GreeterClient(invoker);
            var reply = await client.SayHelloAsync(
              new HelloRequest { Name = "GreeterClient" },deadline: DateTime.UtcNow.AddSeconds(5));
            Console.WriteLine("Greeting: " + reply.Message);

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

            //using var replyStreamClient = client.StreamingClient();

            //foreach (var name in new[] { "Stacking", "Client", "Stream" })
            //{
            //    await replyStreamClient.RequestStream.WriteAsync(new HelloRequest
            //    {
            //        Name = name
            //    });
            //}

            //await replyStreamClient.RequestStream.CompleteAsync();
            //var response = await replyStreamClient.ResponseAsync;

            //Console.WriteLine(response.Count);
            //Console.WriteLine(response.Message);


            //using var replyStreamWays = client.StreamingWays();
            //for (int i = 0; i < 5; i++)
            //{
            //    await replyStreamWays.RequestStream.WriteAsync(new HelloRequest
            //    {
            //        Name = "StreamWaysName " + i,
            //    });
            //}

            //while (await replyStreamWays.ResponseStream.MoveNext())
            //{
            //    try
            //    {
            //        Console.WriteLine($"Response Return:{replyStreamWays.ResponseStream.Current.Message}");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}

            //await replyStreamWays.RequestStream.CompleteAsync();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
