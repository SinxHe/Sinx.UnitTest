using System.Collections;

namespace Orleans.ChatRoom.Grains;

public interface IUserGrain : IGrainWithStringKey
{
	Task SetNameAsync(string name);
	Task<string> GetNameAsync();
	Task SendMessageAsync(IMessageGrain messageGrain, string message);
}
