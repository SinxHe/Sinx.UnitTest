using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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
    }
}
