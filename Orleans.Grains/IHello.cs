using System.Threading.Tasks;

namespace Orleans.Grains
{
	/// <summary>
	/// # 单线程执行
	/// https://docs.microsoft.com/en-us/dotnet/orleans/benefits#single-threaded-execution-of-grains
	/// Orleans运行时保证grains在同一时间只有一个线程在执行。开发永远不用在grain层级考虑并发问题. 永远不用使用锁或者其他同步机制来控制共享数据的访问.
	/// # 透明激活
	/// 运行时只会在需要的时候激活grain。
	/// </summary>
	public interface IHello : IGrainWithIntegerKey
	{
		// grain 的返回值是 Task<T> 或 Task
		Task<string> SayHello(string greeting);
	}
}
