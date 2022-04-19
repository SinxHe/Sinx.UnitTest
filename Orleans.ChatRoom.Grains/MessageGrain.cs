using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Orleans.ChatRoom.Grains;

public class MessageGrain : Grain, IMessageGrain
{
	private readonly IPersistentState<List<string>> _messageState;
	private readonly ObserverManager<IMessageObserver> _subsManager;

	public MessageGrain(
		ILogger<MessageGrain> logger,
		[PersistentState("messages", "sqlite")]IPersistentState<List<string>> messageState)
	{
		_messageState = messageState;
		_subsManager = new(TimeSpan.FromMinutes(5), logger, "subs");
	}

	public Task<IEnumerable<string>> GetMessagesAsync()
	{
		return Task.FromResult(_messageState.State.AsEnumerable());
	}

	public async Task AddMessageAsync(string message)
	{
		_messageState.State.Add(message);
		await _messageState.WriteStateAsync();
		await _subsManager.Notify(s => s.OnMessage(message));
	}
	
	public Task SubscribeAsync(IMessageObserver messageGrain)
	{
		_subsManager.Subscribe(messageGrain, messageGrain);
		return Task.CompletedTask;
	}
	public Task UnsubscribeAsync(IMessageObserver messageGrain)
	{
		_subsManager.Unsubscribe(messageGrain);
		return Task.CompletedTask;
	}
}
