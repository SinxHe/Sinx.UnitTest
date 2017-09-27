using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Xunit;
using Microsoft.Extensions.Configuration.Memory;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
	public class _01IConfigurationProviderTests
	{
		[Fact]
		public void IConfigurationProvider_JsonConfigurationProvider()
		{
			var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
			var source = new JsonConfigurationSource
			{
				FileProvider = fileProvider,
				// 这里应该是相对fileProvider的路径而不是绝对路径, 如果是绝对路径, 即时文件存在且是fileProvider根目录下的也是NotFoundException
				Path = "appsettings.json",
				Optional = false,
				ReloadOnChange = false
			};
			var provider = new JsonConfigurationProvider(source);
			provider.TryGet("Name", out var name0);
			Assert.Null(name0);
			provider.Load();
			provider.TryGet("Name", out var name1);
			Assert.Equal(name1, "SinxHe");
		}

		[Fact]
		public void IConfigurationProvider_MemoryConfigurationProvider()
		{
			var source = new MemoryConfigurationSource
			{
				InitialData = new Dictionary<string, string>
				{
					["Name"] = "SinxHe",
					["ConnectionStrings:Ali"] = "{AliConnectionString}"
				}
			};
			var provider = new MemoryConfigurationProvider(source);
			provider.TryGet("Name", out var name0);
			// !!!
			Assert.Equal(name0, "SinxHe");
			provider.Load();
			provider.TryGet("Name", out var name1);
			Assert.Equal(name1, "SinxHe");
		}

		[Fact]
		public void IConfigurationProvider_EnvironmentVariablesConfigurationProvider()
		{
			Environment.SetEnvironmentVariable("Name", "SinxHe");
			var source = new EnvironmentVariablesConfigurationSource
			{
				Prefix = null
			};
			var provider = new EnvironmentVariablesConfigurationProvider();
			provider.TryGet("Name", out var name);
			Assert.Null(name);
			provider.Load();
			provider.TryGet("Name", out name);
			Assert.Equal(name, "SinxHe");
		}

		[Fact]
		public void IConfigurationProvider_CommandLine()
		{
			var args = new[] { "--name", "SinxHe" };
			var map = new Dictionary<string, string>
			{
				["--name"] = "Name"
			};
			var source = new CommandLineConfigurationSource
			{
				Args = args,
				SwitchMappings = map
			};
			var provider = new CommandLineConfigurationProvider(args, map);
			provider.TryGet("Name", out var name);
			Assert.Null(name);
			provider.Load();
			provider.TryGet("Name", out name);
			Assert.Equal(name, "SinxHe");
		}
	}
}
