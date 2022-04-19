using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Grains.Interface;

try
{
	await using var client = await ConnectClientAsync();
	await DoClientWorkAsync(client);
	Console.ReadKey();
	return 0;
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
	Console.ReadKey();
	return -1;
}

static async Task DoClientWorkAsync(IClusterClient client)
{
	// 在使用全局唯一的Grain的时候, 使用Guid.Empty作为Grain的Id
	// Orleans保证在进行调用的时候集群中有一个Grain的实例, 如果没有, 则会选择一个机器激活一个Grain的实例(Grain Placement)
	// 默认激活策略是随机的, 可以全局配置或者针对Grain对象进行配置
	var grain = client.GetGrain<IHello>(0);
	var result = await grain.SayHello("------------------Hello World!");
	Console.WriteLine(result);
}

static async Task<IClusterClient> ConnectClientAsync()
{
	var client = new ClientBuilder()
		.UseLocalhostClustering()
		.ConfigureLogging(logging => logging.AddConsole())
		.Build();
	await client.Connect();
	Console.WriteLine("Client successfully connected to silo host");

	return client;
}