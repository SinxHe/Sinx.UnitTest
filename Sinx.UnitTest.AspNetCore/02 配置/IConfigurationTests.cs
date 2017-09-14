using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
    public class IConfigurationTests
    {
	    [Fact]    // 配置 Conciguration
	    public void SummaryTest()
	    {
		    // 构建
		    var config = new ConfigurationBuilder()  // Microsoft.Extensions.Configuration.dll
			    .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)    // Microsoft.Extensions.Configuration.FileExtensions.dll
			    .AddJsonFile("appsettings.json")                // Microsoft.Extensions.Configuration.Json.dll
			    .Build();
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
	}
}
