using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;

try
{
	var host = await StartSiloAsync();
	Console.WriteLine("\n\n Press Enter to terminate...\n\n");
	Console.ReadLine();

	await host.StopAsync();

	return 0;
}
catch (Exception ex)
{
	Console.WriteLine(ex);
	return 1;
}

static async Task<IHost> StartSiloAsync()
{
	var builder = new HostBuilder()
		.UseOrleans(c =>
		{
			// ClusterOptions - ClusterId: 集群唯一Id, 集群内的Clients和Silos可以直接进行交流
			//					ServiceId: 应用唯一Id, 在不同的部署中应该保持一致, Providers会使用它, 比如持久化Provider, RedisKey前缀 
			
			// siloPort, silo-to-silo 端口, gatewayPort, client-to-silo 端口
			// .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
			c.UseLocalhostClustering()
				.ConfigureLogging(logging => logging.AddConsole());
			c.AddAdoNetGrainStorage("sqlite", options =>
			{
				options.Invariant = "MySql.Data.MySqlClient";
				options.ConnectionString = "Server=localhost;Port=8080;Database=ChatRoom;Uid=root;Pwd=sinx";
				options.UseJsonFormat = true;
			});
		});

	var host = builder.Build();
	await host.StartAsync();

	return host;
}
