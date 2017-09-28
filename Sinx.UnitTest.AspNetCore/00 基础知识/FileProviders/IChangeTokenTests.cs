using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._00_基础知识.FileProviders
{
    public class IChangeTokenTests
    {
		[Fact]
	    public void IChangeToken_CancellationToken()
	    {
		    System.Threading.CancellationTokenSource tokenSource = new System.Threading.CancellationTokenSource();
		    Assert.False(tokenSource.IsCancellationRequested);
			var isCallbackInvoked = false;
		    tokenSource.Token.Register(() => isCallbackInvoked = true);
		    tokenSource.Cancel();
		    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
		    Assert.True(isCallbackInvoked);
		    Assert.True(tokenSource.Token.IsCancellationRequested);
	    }

		[Fact]
	    public void IChangeToken_ConfigurationReloadToken()
	    {
		    IChangeToken token = new ConfigurationReloadToken();
		    Assert.False(token.HasChanged);
			// token会主动调用CallBack方法
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
    }
}
