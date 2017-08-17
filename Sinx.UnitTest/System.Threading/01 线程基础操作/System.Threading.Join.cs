using System;
using System.Threading;
using Xunit;

namespace Sinx.UnitTest.System.Threading._01_线程基础操作
{
	/// <summary>
	/// 让一个线程等待另一个线程执行完成
	/// </summary>
	public class JoinTest
	{
		[Fact]
		public void Join_WaitAnotherThreadComplete()
		{
			var num = 0;
			var thread = new Thread(() =>
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
				num = 1;
			});
			thread.Start();
			thread.Join();	// 阻止调用线程直到线程终止
			Assert.Equal(num, 1);
		}
	}
}
