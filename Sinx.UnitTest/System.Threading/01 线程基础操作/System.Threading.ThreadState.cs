using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Threading._01_线程通信和同步
{
	/// <summary>
	/// 获取线程状态
	/// </summary>
	public class ThreadStateTest
	{
		[Fact]
		public void Thread_ThreadState()
		{
			var thread = new Thread(() => Thread.Sleep(TimeSpan.FromSeconds(2)));
			Assert.Equal(thread.ThreadState, ThreadState.Unstarted);
			thread.Start();
			Assert.Equal(thread.ThreadState, ThreadState.Running);
			Thread.Sleep(TimeSpan.FromMilliseconds(500));
			Assert.Equal(thread.ThreadState, ThreadState.WaitSleepJoin);
			Thread.Sleep(TimeSpan.FromMilliseconds(2500));
			Assert.Equal(thread.ThreadState, ThreadState.Stopped);
			thread.Abort();
			Assert.Equal(thread.ThreadState, ThreadState.Aborted);

			//thread.Start();	// 线程正在运行或已被终止, 因此无法运行
			thread = new Thread(() => { });
			thread.Start();
			Thread.Sleep(TimeSpan.FromSeconds(2));
			//Assert.Equal(thread.ThreadState, ThreadState.WaitSleepJoin);
		}
	}
}
