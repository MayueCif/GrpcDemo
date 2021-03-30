using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServiceDemo
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task StreamingServer(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new HelloReply() { Message= "Hello From "+ request.Name });
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }

        public override async Task<HelloReply> StreamingClient(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var replay = new HelloReply();
            while (await requestStream.MoveNext())
            {
                replay.Count++;
                replay.Message = "Hello From " + requestStream.Current.Name;
            }
            return replay;
        }

        public override async Task StreamingWays(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
                    
            while (await requestStream.MoveNext())
            {
                Console.WriteLine($"Begin read request");
                Console.WriteLine(requestStream.Current.Name);
                await responseStream.WriteAsync(new HelloReply() { 
                    Message=DateTimeOffset.Now.ToString("HH:mm:ss")
                });
            }

        }

        /// <summary>
        /// �ٷ�д��ʾ��
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task Official(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context) 
        {
            // Read requests in a background task.         
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    //Process request.
                }
            });

            // Send responses until the client signals that it is complete.
            while (!readTask.IsCompleted)
            {
                await responseStream.WriteAsync(new HelloReply());
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }

    }
}
