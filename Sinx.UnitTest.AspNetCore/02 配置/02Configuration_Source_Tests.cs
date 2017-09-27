using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
    public class _02Configuration_Source_Tests
    {
	    private readonly ConfigurationBuilder _configurationBuilder;
		public _02Configuration_Source_Tests()
		{
			_configurationBuilder = new ConfigurationBuilder();
		}

		[Fact]
	    public void Configuration_Source_JsonFile_Test()
	    {
		    var conf = _configurationBuilder
			    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			    .Build();
		    string name = conf["Name"];
		    Assert.Equal(name, "SinxHe");
		}

		[Fact]
	    public void Configuration_Source_Memory_Test()
	    {
		    var kvs = new Dictionary<string, string>
		    {
			    ["Name"] = "SinxHe"
		    };
		    var conf = _configurationBuilder
			    .AddInMemoryCollection(kvs)
				.Build();
		    string name = conf["Name"];
		    Assert.Equal(name, "SinxHe");
	    }

		[Fact]
	    public void Configuration_Source_EnvironmentVariable()
		{
			Environment.SetEnvironmentVariable("Name", "SinxHe");
			Environment.SetEnvironmentVariable("ConnectionStrings:Ali", "AliConnectionString");
			var conf = _configurationBuilder
				.AddEnvironmentVariables()
				.Build();
			var name = conf["Name"];
			Assert.Equal(name, "SinxHe");
			var connStr = conf.GetSection("ConnectionStrings").GetSection("Ali").Value;
			Assert.Equal(connStr, "AliConnectionString");
		}

		[Fact]
	    public void Configuration_Source_CommondLine()
		{
			var args = new[] { "-n", "SinxHe"};
			var map = new Dictionary<string, string>
			{
				{ "-n", "Name"}
			};
			var conf = _configurationBuilder
				.AddCommandLine(args, map)
				.Build();
			var name = conf["Name"];
			Assert.Equal(name, "SinxHe");
		}
    }
}
