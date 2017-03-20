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

		[Fact]
		public void CancellationToken_None_CanNotCancel()
		{
			var ct = new CancellationToken();
			ct = CancellationToken.None;	// None - 不允许被取消的操作, 不跟任何CancellationTokenSource关联
			var b = true;
			ThreadPool.QueueUserWorkItem(state =>
			{
				while (true)
				{
					if (ct.IsCancellationRequested)	// 总是返回false
					{
						break;
					}
					b = ct.CanBeCanceled;	// 指示为不能进行取消的操作
				}
			});
			Thread.Sleep(3000);
			Assert.False(b);
		}

		[Fact]
		public void CancellationTokenSource_CallBackImp()
		{
			var cts = new CancellationTokenSource();
			var ar = new[] {0};
			var are = new AutoResetEvent(false);
			ThreadPool.QueueUserWorkItem(a =>
			{
				for (var i = 0; !cts.IsCancellationRequested; i++)
				{
					if (i == 100)
					{
						cts.Token.Register(state =>
						{
							ar[0] = 100;
						}, ar, false);    // 一个状态,一个指明是否使用上下文的bool
						// false: 调用Cancel的线程会顺序调用登记的所有方法
						are.Set();
						Thread.Sleep(1000); // 等待确保判断100的断言执行
					}
					if (i == 1000)
					{
						cts.Token.Register(state =>
						{
							ar[0] = 1;
						}, ar, true);
						// true: 回调会被send而不是post给已捕捉的SynchronizationContext, 后者决定哪个线程调用回调方法
						are.Set();
					}
				}
			}, are);
			are.WaitOne();
			Assert.NotEqual(100, ar[0]);
			are.Reset();
			are.WaitOne(); 
		}
	}
}
