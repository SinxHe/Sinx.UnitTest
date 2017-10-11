using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._00_基础知识.FileProviders
{
    public class IChangeTokenTests
    {
		[Fact]
	    public void IChangeToken_ConfigurationReloadToken()
	    {
		    IChangeToken token = new ConfigurationReloadToken();
		    Assert.False(token.HasChanged);
		    Assert.True(token.ActiveChangeCallbacks);	
	    }

		[Fact]
	    public void IChangeToken_ConfigurationReloadToken_OnChange()
		{
			var isChangeCallbackInvoke = false;
			IChangeToken token = new ConfigurationReloadToken();
			token.RegisterChangeCallback(o => isChangeCallbackInvoke = true, null);
			Assert.False(token.HasChanged);
			Assert.False(isChangeCallbackInvoke);
			((ConfigurationReloadToken) token).OnReload();
			Assert.True(token.HasChanged);
			Assert.True(isChangeCallbackInvoke);
		}

	    [Fact]
	    public void IChangeToken_ConfigurationReloadToken_CallCancleAgain()
	    {
		    var token = new ConfigurationReloadToken();
		    var isCallbackInvoked = false;
			token.RegisterChangeCallback(o => isCallbackInvoked = true, null);
			token.OnReload();
		    Wait();
			Assert.True(isCallbackInvoked);
			Assert.True(token.HasChanged);

			isCallbackInvoked = false;
			var isCallbackInvokedAgain = false;
		    token.RegisterChangeCallback(o => isCallbackInvokedAgain = true, null);
		    Assert.True(token.HasChanged);
		    Wait();
			token.OnReload();
			// 还是会调用第二次
		    Assert.True(isCallbackInvokedAgain);
			// 新注册的回调还是会调用的
		    Assert.True(isCallbackInvokedAgain);
	    }

	    [Fact]
	    public void IChangeToken_CancellationToken_CallCancleAgain()
	    {
		    var tokenSource = new CancellationTokenSource();
		    Assert.False(tokenSource.IsCancellationRequested);
		    var isCallbackInvoked = false;
		    tokenSource.Token.Register(() => isCallbackInvoked = true);
		    tokenSource.Cancel();
			Wait();
			Assert.True(isCallbackInvoked);
		    Assert.True(tokenSource.Token.IsCancellationRequested);

		    // 已经cancel的token再次cancel不会触发回调
		    isCallbackInvoked = false;
		    var isCallbackInvokedAgain = false;
		    tokenSource.Token.Register(() => isCallbackInvokedAgain = true);
		    Assert.True(tokenSource.Token.IsCancellationRequested);
		    tokenSource.Cancel();
		    Wait();
			// 已经执行过的不会再执行
			Assert.False(isCallbackInvoked);
			// 新注册的回调还是可以执行的
		    Assert.True(isCallbackInvokedAgain);
	    }

		[Fact]
		public void IChangeToken_CancellationToken_ExchangeTokenBeforeOnRelaod()
		{
			// - 回调本该执行一次, 但是这里调用了两次, 说明同一个token被绑定上了两次而不是在每一个新token上绑定 -
			var token = new ConfigurationReloadToken();
			int times = 0;
			ChangeToken.OnChange(() => token, () =>
			{
				token = new ConfigurationReloadToken();
				times++;
			});
			token.OnReload(); // -> 2次
			Assert.Equal(2, times);
			token.OnReload(); // -> 0次
			Assert.Equal(2, times);
			// - 正确的调用一次的方式 -
			var t = new ConfigurationReloadToken();
			var count = 0;
			ChangeToken.OnChange(() => t, () =>
			{
				count++;
			});
			Interlocked.Exchange(ref t, new ConfigurationReloadToken()).OnReload();
			Assert.Equal(1, count);
			Interlocked.Exchange(ref t, new ConfigurationReloadToken()).OnReload();
			Assert.Equal(2, count);
		}

	    private void Wait()
	    {
		    Thread.Sleep(TimeSpan.FromSeconds(2));
	    }
    }
}
