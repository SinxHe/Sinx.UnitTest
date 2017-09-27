using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Primitives;

namespace Sinx.UnitTest.AspNetCore._00_基础知识.FileProviders
{
	public class PhysicalFileProviderTests
	{
		private readonly string _appBasePath = PlatformServices.Default.Application.ApplicationBasePath;

		[Fact]
		public void PhysicalFileProvider_MoniteFile_MoniteOneTime()
		{
			var isCallBackInvoke = false;
			var provider = new PhysicalFileProvider(_appBasePath);
			IChangeToken token = provider.Watch("appsettings.json");
			// 没有进行文件更改
			token.RegisterChangeCallback(_ => isCallBackInvoke = true, null);
			Task.Delay(TimeSpan.FromSeconds(2)).Wait();	// 等待回调执行完成
			Assert.False(isCallBackInvoke);
			Assert.False(token.HasChanged);
			// 进行了文件更改
			File.WriteAllText(Path.Combine(provider.Root, "appsettings.json"), DateTime.Now.ToString(CultureInfo.InvariantCulture));
			Task.Delay(TimeSpan.FromSeconds(2)).Wait();	// 等待回调执行完成
			Assert.True(isCallBackInvoke);
			Assert.True(token.HasChanged);
			// 只能监控一次
			isCallBackInvoke = false;
			File.WriteAllText(Path.Combine(provider.Root, "appsettings.json"), DateTime.Now.ToString(CultureInfo.InvariantCulture));
			Task.Delay(TimeSpan.FromSeconds(2)).Wait(); // 等待回调执行完成
			Assert.False(isCallBackInvoke);	// !!
			Assert.True(token.HasChanged);

			// Token.HasChanged没有set方法, 所以只能监控一次
			//token.HasChanged = false;
		}

		[Fact]
		public async Task PhysicalFileProvider_MoniteFile_MoniteTwoTime()
		{
			var provider = new PhysicalFileProvider(_appBasePath);
			var isCallBackInvoked = false;
			var token = provider.Watch("appsettings.json");
			token.RegisterChangeCallback(_ => isCallBackInvoked = true, null);
			await File.WriteAllTextAsync(Path.Combine(provider.Root, "appsettings.json"), DateTime.Now.ToString(CultureInfo.InvariantCulture));
			Assert.True(isCallBackInvoked);
			Assert.True(token.HasChanged);

			isCallBackInvoked = false;
			token = provider.Watch("appsettings.json");
			token.RegisterChangeCallback(_ => isCallBackInvoked = true, null);
			await File.WriteAllTextAsync(Path.Combine(provider.Root, "appsettings.json"), DateTime.Now.ToString(CultureInfo.InvariantCulture));
			Assert.True(isCallBackInvoked);
			Assert.True(token.HasChanged);
		}

		[Fact]
		public async Task PhysicalFileProvider_MoniteFile_MoniteTwoTime_OnChange()
		{
			var provider = new PhysicalFileProvider(_appBasePath);
			var isCallBackInvoked = false;
			ChangeToken.OnChange(() => provider.Watch("appsettings.json"), () => isCallBackInvoked = true);
			await File.WriteAllTextAsync(Path.Combine(provider.Root, "appsettings.json"), DateTime.Now.ToString(CultureInfo.InvariantCulture));
			Assert.True(isCallBackInvoked);

			// 第二次监控
			isCallBackInvoked = false;
			await File.WriteAllTextAsync(Path.Combine(provider.Root, "appsettings.json"), DateTime.Now.ToString(CultureInfo.InvariantCulture));
			Assert.True(isCallBackInvoked);

		}
	}
}
