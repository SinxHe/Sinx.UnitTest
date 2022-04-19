using System;
using System.Threading.Tasks;
using Orleans.ChatRoom.Grains;

namespace Orleans.ChatRoom.Client;

public class MessageObserver : IMessageObserver
{
	public Task OnMessage(string message)
	{
		Console.WriteLine(message);
		return Task.CompletedTask;
	}
}
