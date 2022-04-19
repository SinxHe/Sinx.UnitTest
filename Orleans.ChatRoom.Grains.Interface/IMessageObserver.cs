namespace Orleans.ChatRoom.Grains;

public interface IMessageObserver : IGrainObserver
{
	Task OnMessage(string message);
}
