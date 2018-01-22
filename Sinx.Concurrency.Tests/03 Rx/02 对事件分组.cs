using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Concurrency.Tests._03_Rx
{
    public class _02_对事件分组
    {
		/// <summary>
		/// Buffer 对组内进行分组, 但是等组内都完成再组装成集合发布
		/// </summary>
		/// <returns></returns>
		[Fact]
	    public async Task IObservable_Buffer()
		{
			IObservable<IList<long>> observable = Observable.Interval(TimeSpan.FromSeconds(1))
				.Buffer(2);
			IDisposable dis = observable.Subscribe(e => 
				Console.WriteLine(DateTime.Now.Second + 
				                  $": Got {e[0]} and {e[1]}"));
			Console.WriteLine("Not Finished");
			//dis.Dispose(); // dispose以后就不会显示了
			await observable;
			Console.WriteLine("Finished");
			dis.Dispose();
		}

		/// <summary>
		/// Windows 对组内进行分组, 但是在每个事件到达的时候就发布
		/// </summary>
		/// <returns></returns>
		[Fact]
	    public async Task IObservable_Window()
	    {
		    IObservable<IObservable<long>> observable = Observable.Interval(TimeSpan.FromSeconds(1))
			    .Window(2);
		    observable
			    .Subscribe(g =>
			    {
				    Console.WriteLine("Start new"); 
				    g.Subscribe(e => Console.WriteLine(e.ToString()),
					    () => Console.WriteLine("OnComplete"));
			    });
		    await observable;
	    }
    }
}
