using System;
using System.Threading;
using Xunit;

namespace Sinx.UnitTest.System.Threading._01_线程基础操作
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
			Assert.Equal(ThreadState.Unstarted, thread.ThreadState);
			thread.Start();
			Assert.Equal(ThreadState.Running, thread.ThreadState);
			Thread.Sleep(TimeSpan.FromMilliseconds(500));
			Assert.Equal(ThreadState.WaitSleepJoin, thread.ThreadState);
			Thread.Sleep(TimeSpan.FromMilliseconds(2500));
			Assert.Equal(ThreadState.Stopped, thread.ThreadState);
			thread.Abort();
			Assert.Equal(ThreadState.Aborted, thread.ThreadState);

			//thread.Start();	// 线程正在运行或已被终止, 因此无法运行
			thread = new Thread(() => { });
			thread.Start();
			Thread.Sleep(TimeSpan.FromSeconds(2));
			//Assert.Equal(thread.ThreadState, ThreadState.WaitSleepJoin);
		}
	}
}
