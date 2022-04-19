using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.ChatRoom.Client;
using Orleans.ChatRoom.Grains;
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
	var messageGrain = client.GetGrain<IMessageGrain>(Guid.Empty);
	Console.WriteLine("Enter your name: ");
	var name = Console.ReadLine();
	var userGrain = client.GetGrain<IUserGrain>(name);
	await userGrain.SetNameAsync(name);
	var messageObserver = new MessageObserver();
	var messageObserverRef = await client.CreateObjectReference<IMessageObserver>(messageObserver);
	await messageGrain.SubscribeAsync(messageObserverRef);
	Console.WriteLine(string.Join("\n", await messageGrain.GetMessagesAsync()));
	while (true)
	{
		var message = Console.ReadLine()!;
		await userGrain.SendMessageAsync(messageGrain, message);
	}
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
