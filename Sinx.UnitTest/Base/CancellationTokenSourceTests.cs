using System.Threading;
using Xunit;

namespace Sinx.UnitTest.Base
{
	public class CancellationTokenSourceTests
	{
		[Fact]
		public void CancellationTokenSource_Thread4Callback()
		{
			int GetThreadId() => Thread.CurrentThread.ManagedThreadId;
			var threadId = GetThreadId();
			var token = new CancellationTokenSource();
			var callThreadId = 0;
			token.Token.Register(() =>
			{
				callThreadId = GetThreadId();
			});
			token.Cancel();
			// 调用Cancel以后, 调用线程去执行注册的事件
			Assert.NotEqual(threadId, 0);
			Assert.Equal(callThreadId, threadId);
		}
	}
}
