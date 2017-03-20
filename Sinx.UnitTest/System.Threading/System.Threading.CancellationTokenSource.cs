using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Threading
{
	public class CancellationTokenSourceTests
	{
		[Fact]
		public void CancellationTokenSource_Cancel()
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			AutoResetEvent are = new AutoResetEvent(false);
			ThreadPool.QueueUserWorkItem(state =>
			{
				while (true)
				{
					if (!cts.IsCancellationRequested) continue;
					are.Set();
					break;
				}
			});
			Thread.Sleep(100);	// 等待一会, 保证上面的continue代码也能执行
			cts.Cancel();
			are.WaitOne();	// 如果没有取消成功的话, 这里会一直等下去
		}
	}
}
