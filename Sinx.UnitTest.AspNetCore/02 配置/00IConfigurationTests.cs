using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
	public class _00IConfigurationTests
	{
		private readonly IConfiguration _config;
		public _00IConfigurationTests()
		{
			// 构建
			var config = new ConfigurationBuilder()  // Microsoft.Extensions.Configuration.dll
				.SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)    // Microsoft.Extensions.Configuration.FileExtensions.dll
				.AddJsonFile("appsettings.json")                // Microsoft.Extensions.Configuration.Json.dll
				.Build();
			_config = config;
		}

		[Fact]    // 配置 Conciguration
		public void SummaryTest()
		{
			// 构建
			var config = _config;
			// 使用
			var connStr0 = config.GetSection("ConnectionStrings").GetSection("Ali").Value;
			var connStr1 = config["ConnectionStrings:Ali"];
			var connStr2 = config.GetConnectionString("Ali");
			var dadName = config["Family:0:Name"];
			// 断言
			Assert.True(connStr0 == connStr1);
			Assert.True(connStr1 == connStr2 && connStr2 != null);
			Assert.True(!string.IsNullOrWhiteSpace(dadName));
		}

		[Fact]
		public void IConfiguration_Dic_SectionToValue_Null()
		{
			var config = _config;
			var connStr = config["ConnectionStrings"];
			Assert.Null(connStr);
			// --------- 与 Dic 不一样 -------
			var dic = new Dictionary<string, string> { ["Age"] = "22" };
			Assert.Throws<KeyNotFoundException>(() => dic["Name"]);
		}

		[Fact]
		public void IConfiguration_Dic_KeyNotFound_Null()
		{
			var config = _config;
			var notExistValue = config["xxxxx"];
			Assert.Null(notExistValue);
		}

		[Fact]
		public void IConfiguration_GetSection_ValueToSection_ValuePropertyIsNull()
		{
			var config = _config;
			var connStr = config.GetSection("ConnectionStrings");
			Assert.Null(connStr.Value);
		}

		[Fact]
		public void IConfiguration_GetSection_KeyNotFound_ValuePropertyIsNull()
		{
			var xxx = _config.GetSection("xxx");
			Assert.Null(xxx.Value);
		}

		/// <summary>
		/// 备注: 不妨称Value不为null的为KVSection, Value为null的ObjectSection
		/// </summary>
		[Fact]
		public void IConfiguration_GetChildren_KVSection_Empty()
		{
			var sections = _config.GetSection("Name").GetChildren();
			Assert.Empty(sections);
		}

		[Fact]
		public void IConfiguration_GetChilden_UseOnPathSectionAndChildenIncludeKVSectionAndPathSection_PathSectionValueIsNullKVSectionValueIsNotNull()
		{
			var sections = _config.GetChildren().ToList();
			Assert.NotNull(sections.Single(e => e.Key == "Name").Value);
			Assert.Null(sections.Single(e => e.Key == "Family").Value);
			Assert.Null(sections.Single(e => e.Key == "ConnectionStrings").Value);
		}

		/// <summary>
		/// 在reload以后, 并不是新创建了一个Configuration, 而是更改了原来的Configuration的值, 还有Token整个对象都是新的, 原来的Token的HasChanged是true, 新的是false
		/// </summary>
		[Fact]
		public void IConfiguration_GetReloadToken()
		{
			var reloadToken = _config.GetReloadToken();
			var name = _config["Name"];
			Assert.NotNull(name);
			Assert.Equal(reloadToken, _config.GetReloadToken());
			Assert.False(reloadToken.HasChanged);
			Assert.False(_config.GetReloadToken().HasChanged);
			var confRoot = _config as IConfigurationRoot;
			Assert.Same(confRoot, _config);
			Assert.Equal(reloadToken, confRoot?.GetReloadToken());
			Assert.False(reloadToken.HasChanged);
			Assert.False(confRoot?.GetReloadToken().HasChanged);
			confRoot?.Reload();
			Assert.Same(confRoot, _config);	// confRoot没有改变, 改变的只是里面的值, 和Token的值
			Assert.NotEqual(reloadToken, confRoot?.GetReloadToken());
			Assert.NotSame(reloadToken, confRoot?.GetReloadToken());
			Assert.True(reloadToken.HasChanged);
			Assert.False(confRoot?.GetReloadToken().HasChanged);
		}
	}
}
