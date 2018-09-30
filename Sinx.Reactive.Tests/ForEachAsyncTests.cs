using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Reactive.Tests
{
	public class ForEachAsyncTests
	{
		/// <summary>
		/// ForEachAsync向Observable注册一个Observer并开始订阅, 并返回一个可等待的Task
		/// </summary>
		[Fact]
		public async Task Observable_ForEachAsync()
		{
			async Task CreateAndConsumeAction(IObserver<int> o)
			{
				for (int i = 0; i < 3; i++)
				{
					await Task.Delay(TimeSpan.FromSeconds(1));
					$"Created {i}".Dump();
					o.OnNext(i);
					$"Consumed {i}".Dump();
					"----------------".Dump();
				}
			}
			var observable = Observable.Create((Func<IObserver<int>, Task>) CreateAndConsumeAction);
			await observable.ForEachAsync(e => $"Consuming {e}".Dump());
			"★★★★★Finished★★★★★".Dump();
		}
	}
}
