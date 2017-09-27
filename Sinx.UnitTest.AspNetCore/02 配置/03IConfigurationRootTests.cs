using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
    public class _01IConfigurationRootTests
    {
		private readonly IConfigurationRoot _configRoot;
		public _01IConfigurationRootTests()
		{
			_configRoot = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();
		}

		[Fact]
	    public void IConfigurationRoot_Reload()
		{
			var configRootTemp = _configRoot;
			var token1 = _configRoot.GetReloadToken();
			Assert.False(token1.HasChanged);
			_configRoot.Reload();
			var token2 = _configRoot.GetReloadToken();
			Assert.NotSame(token1, token2);
			Assert.Same(configRootTemp, _configRoot);
			Assert.True(token1.HasChanged);
			Assert.False(token2.HasChanged);
		}

		/// <summary>
		/// Provider 最终是通过 IDictionary<string, string>跟IConfigurationProvider联系上的, 层级关系通过Key中添加":"来实现
		/// </summary>
		[Fact]
	    public void IConfigurationRoot_Providers_UseIDictionaryConnectWithConfigurationProvider()
	    {
			var jsonProvider = _configRoot.Providers.Single(e => e.GetType() == typeof(JsonConfigurationProvider));
		    var isGet = jsonProvider.TryGet("Name", out var name);	// 其实内部调用的是Dictionary<string, string>的TryGet
		    Assert.Equal(name, "SinxHe");
		    Assert.True(isGet);

		    isGet = jsonProvider.TryGet("Ali", out var connStr);
		    Assert.Null(connStr);
		    Assert.False(isGet);

		    isGet = jsonProvider.TryGet("ConnectionStrings:Ali", out connStr);
		    Assert.NotNull(connStr);
		    Assert.True(isGet);
	    }

		[Fact]
	    public void IConfigurationRoot_Providers_MemoryProvider()
		{
			var dic = new Dictionary<string, string>
			{
				["ConnectionStrings:Ali"] = "{AliConnectionString}",
				["Name"] = "SinxHe",
				["Family:0:Name"] = "Brother"
			};
			var conf = new ConfigurationBuilder()
				.AddInMemoryCollection(dic)
				.Build();
			var name = conf.GetSection("Name").Value;
			Assert.Equal(name, "SinxHe");
			var connStr = conf.GetSection("ConnectionStrings").GetSection("Ali").Value;
			Assert.NotNull(connStr);
			var brother = conf.GetSection("Family").GetChildren().First().GetSection("Name").Value;
			Assert.Equal(brother, "Brother");
		}

		[Fact]
	    public void IConfigurationRoot_Reload_JsonFile_DirectlyGet()
		{
			var path = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "appsettings.json");
			var conf = new ConfigurationBuilder()
				.AddJsonFile(path, false, true)
				.Build();
			var name = conf["Name"];
			Assert.Equal(name, "SinxHe");

			var content = File.ReadAllText(path);
			File.WriteAllText(path, content.Replace("SinxHe", "DaXiong"));

			name = conf["Name"];
			Assert.NotEqual(name, "DaXiong");
			conf.Reload();
			// 使用另一个线程进行重载, 所以这个时候要等一下, 等他重载完成
			// 如果没有重载完成就进行获取, 会获取到null
			Task.Delay(TimeSpan.FromSeconds(2)).Wait();
			name = conf["Name"];
			Assert.Equal(name, "DaXiong");
		}
    }
}
