using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Sinx.Grpc.Server;

namespace Sinx.Grpc.Client
{
	internal static class Program
	{
		private static async Task Main()
		{
			// 创建一个通道, 创建通道成本高昂, 尽量重用.
			var channel = GrpcChannel.ForAddress("https://localhost:5001");
			// 使用通道创建一个客户端, 一个通道可以创建多个客户端
			// 客户端是轻型对象, 无需缓存或重用
			// 客户端可以同时进行多个调用
			var client = new Greeter.GreeterClient(channel);
			await RunAsync(client);
			await Run2Async(client);
			await Run3Async(client);
			await Run4Async(client);
		}

		private static async Task Run4Async(Greeter.GreeterClient client)
		{
			using var call = client.ClientServerStream();
			Console.WriteLine("Starting background task to receive messages");
			var readTask = Task.Run(async () =>
			{
				await foreach (var response in call.ResponseStream.ReadAllAsync())
				{
					Console.WriteLine(response.Message);
					// Echo messages sent to the service
				}
			});

			Console.WriteLine("Starting to send messages");
			Console.WriteLine("Type a message to echo then press enter.");
			while (true)
			{
				var result = Console.ReadLine();
				if (string.IsNullOrEmpty(result))
				{
					break;
				}

				await call.RequestStream.WriteAsync(new () { Name = result });
			}

			Console.WriteLine("Disconnecting");
			await call.RequestStream.CompleteAsync();
			await readTask;
		}

		// 客户端流式处理调用
		private static async Task Run3Async(Greeter.GreeterClient client)
		{
			using var call = client.ClientStream();
			for (var i = 0; i < 3; i++)
			{
				await call.RequestStream.WriteAsync(new() {Name = $"sinx{i}"});
			}
			await call.RequestStream.CompleteAsync();
			var res = await call;
			Console.WriteLine(res.Message);
		}

		// 服务器流式处理调用
		private static async Task Run2Async(Greeter.GreeterClient client)
		{
			using var call = client.ServerStream(new HelloRequest() { Name = "World" });
			await foreach (var response in call.ResponseStream.ReadAllAsync())
			{
				Console.WriteLine(response.Message);
			}
		}

		// 一元调用
		private static async Task RunAsync(Greeter.GreeterClient client)
		{
			// 客户端将处理消息序列化, 并为gRPC调用寻址到正确服务
			var response = await client.SayHelloAsync(new HelloRequest { Name = "World" });
			Console.WriteLine(response.Message);
		}
	}
}
