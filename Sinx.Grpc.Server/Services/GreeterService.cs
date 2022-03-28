using System.Text;
using Grpc.Core;

namespace Sinx.Grpc.Server.Services;

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

	public override async Task ServerStream(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
	{
		// 可以设计为永久运行
		for (var i = 0; i < 5 || context.CancellationToken.IsCancellationRequested; i++)
		{
			await responseStream.WriteAsync(new()
			{
				Message = "hello " + i
			});
			await Task.Delay(TimeSpan.FromSeconds(1));
		}

		// 方法返回, 服务器流式处理完毕
	}

	public override async Task<HelloReply> ClientStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
	{
		var sb = new StringBuilder();
		while (await requestStream.MoveNext())
		{
			var message = requestStream.Current;
			sb.AppendLine(message.Name + " ");
		}
		sb.AppendLine();
		await foreach (var message in requestStream.ReadAllAsync())
		{
			sb.AppendLine(message.Name);
		}
		return new()
		{
			Message = sb.ToString()
		};
	}


	// 可以支持更复杂的方案，例如同时读取请求和发送响应
	public override async Task ClientServerStream(
		IAsyncStreamReader<HelloRequest> requestStream,
		IServerStreamWriter<HelloReply> responseStream,
		ServerCallContext context)
	{
		var counter = 0;
		await foreach (var message in requestStream.ReadAllAsync())
		{
			counter++;
			await responseStream.WriteAsync(new()
			{
				Message = message + " " + counter
			});
		}
	}
}
