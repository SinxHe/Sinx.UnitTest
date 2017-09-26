using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
    public class _01Configuration_Source_Tests
    {
	    private readonly ConfigurationBuilder _configurationBuilder;
		public _01Configuration_Source_Tests()
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
    }
}
