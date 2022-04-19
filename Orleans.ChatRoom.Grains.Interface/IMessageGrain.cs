namespace Orleans.ChatRoom.Grains;

public interface IMessageGrain : IGrainWithGuidKey
{
	Task<IEnumerable<string>> GetMessagesAsync();
	
	Task AddMessageAsync(string message);
	
	Task SubscribeAsync(IMessageObserver messageGrain);
	
	Task UnsubscribeAsync(IMessageObserver messageGrain);
}
