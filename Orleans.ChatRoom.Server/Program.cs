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
