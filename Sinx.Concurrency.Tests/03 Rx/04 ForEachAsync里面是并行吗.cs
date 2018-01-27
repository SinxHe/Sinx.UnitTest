using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Concurrency.Tests._03_Rx
{
    public class _04_ForEachAsync里面是并行吗
    {
		[Fact]
	    public async Task Observable_ForEachAsync_OneThread()
		{
			var observable = Observable.Create<int>(async o =>
			{
				var collection = Enumerable.Range(0, 10000);
				foreach (var item in collection)
				{
					var num = await Task.FromResult(item);
					o.OnNext(num);
				}
			});
			await observable.ForEachAsync(n =>
			{
				Thread.Sleep(500);
				Console.WriteLine(n);
			});
		}
	    [Fact]
	    public async Task Observable_ForEachAsync_MultipleThread()
	    {
		    var observable = Observable.Create<int>(async o =>
		    {
			    var collection = Enumerable.Range(0, 10000);
			    foreach (var item in collection)
			    {
				    var num = await Task.FromResult(item);
					// 这样在observable释放以后会导致消息发送不出去, 等待ovservable也没用, 不是一个好的设计
				    Task.Run(() =>
				    {
					    o.OnNext(num);
				    }).ContinueWith(t =>
				    {
						if (!t.IsCompletedSuccessfully)
						{
							o.OnError(t.Exception);
						}
				    });
			    }
		    });
		    await observable.ForEachAsync(n =>
		    {
			    Thread.Sleep(500);
			    Console.WriteLine(n);
		    });
	    }
    }
}
