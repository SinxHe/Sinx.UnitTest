using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Concurrency.Tests._01_同步上下文
{
    public class SinxSynchronizeInvokeTests
    {
		[Fact]
	    public void SinxSynchronizeInvoke_Invoke()
		{
			Console.WriteLine($"MainThreadId: {Thread.CurrentThread.ManagedThreadId}");
			var invoke = new SinxSynchronizeInvoke
			{
				Delegate = () =>
				{
					Console.WriteLine($"CurrentThreadId: {Thread.CurrentThread.ManagedThreadId}");
				}
			};

			var t = Task.Run(() => { invoke.Delegate(); });
			t.Wait();
		}
    }
}
