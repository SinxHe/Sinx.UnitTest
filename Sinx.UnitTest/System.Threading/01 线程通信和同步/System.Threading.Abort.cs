using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Threading._01_线程通信和同步
{
	public class ThreadAbortTest
	{
		[Fact]
		public void Thread_Abort_AbortAThread()
		{
			int n = 0;
			var thread = new Thread(() => n++);
			thread.Start();
			Thread.Sleep(TimeSpan.FromSeconds(1));
			thread.Abort();	// 引发 ThreadAbortException 异常
			// 调用线程调用 Abort 方法, 给被调用线程注入了 ThreadAbortException 异常
			// 这非常危险, 因为异常可以在任何时间发生, 这可能会彻底摧毁引用程序
			// 异常也不一定总能被终止, 目标线程可以通过处理该异常并调用Thread.ResetAbort方法来拒绝被终止
			// 使用CancellationToken来取消线程执行比较好
		}
	}
}
