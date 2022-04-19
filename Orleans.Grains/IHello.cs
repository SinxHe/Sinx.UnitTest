using System.Threading.Tasks;

namespace Orleans.Grains.Interface
{
	public interface IHello : IGrainWithIntegerKey
	{
		// grain 的返回值是 Task<T> 或 Task
		Task<string> SayHello(string greeting);
	}
}
