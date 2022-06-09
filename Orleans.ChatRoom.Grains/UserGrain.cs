using Orleans.Runtime;

namespace Orleans.ChatRoom.Grains;

public class UserGrain : Grain, IUserGrain
{

	private readonly IPersistentState<UserState> _userState;
	public UserGrain(
		// grain state is not loaded at the time it is injected into constructor, 
		// so accessing it is invalid at that time, the state will be loaded before `OnActiveAsync` is called.
		[PersistentState("UserState", "sqlite")] IPersistentState<UserState> userState)
	{
		// 注入的时候并没有读取, 在OnActivateAsync调用前保证是读取了的
		_userState = userState;
	}

	public async Task SetNameAsync(string name)
	{
		_userState.State.Name = name;
		await _userState.WriteStateAsync();
	}

	public Task<string> GetNameAsync()
	{
		return Task.FromResult(_userState.State.Name);
	}

	public Task SendMessageAsync(IMessageGrain messageGrain, string message)
	{
		return messageGrain.AddMessageAsync($"{_userState.State.Name} says: {message}");
	}


	[Serializable]
	public class UserState
	{
		public string Name { get; set; } = string.Empty;
	}
}
