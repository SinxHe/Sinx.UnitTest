using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
    public class _05IOptionTests
    {
		private readonly IConfiguration _conf;
		private readonly IServiceCollection _services;

		public _05IOptionTests()
		{
			_conf = new ConfigurationBuilder()
				.SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
				.AddJsonFile("appsettings.json")
				.Build();
			_services = new ServiceCollection();
		}

		[Fact]
	    public void IOption_AddIOptions()
		{
			// Adds services required for using options.
			_services.AddOptions();
			// Microsoft.Extensions.Options.dll
			// Microsoft.Extensions.DependencyInjection.OptionsServiceCollectionExtensions
			// 将配置类通过Configure传递进IConfigureOptions<TOptions>中
			_services.Configure<MyOption>(opt =>
			{
				opt.Name = "daxiong";
			});
			// 注入使用IOptions<TOptions>的类, 会被传递进构造函数
			_services.AddSingleton<UseMyOption>();
			var sp = _services.BuildServiceProvider();
			var useMyOption = sp.GetService<UseMyOption>();
			Assert.Equal("daxiong", useMyOption.Option.Name);
		}

		[Fact]
	    public void IOptoin_AddIOptionsThatBindJsonFile()
	    {
			// 添加IOptions服务 [重要]
		    _services.AddOptions();
			// Microsoft.Extensions.Options.ConfigurationExtensions.dll
			// Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions
			_services.Configure<MyOption>(_conf);
		    _services.AddSingleton<UseMyOption>();
		    var sp = _services.BuildServiceProvider();
		    var useMyOption = sp.GetService<UseMyOption>();
		    Assert.Equal("SinxHe", useMyOption.Option.Name);
		    Assert.Equal(24, useMyOption.Option.Age);
	    }

	    public class UseMyOption
	    {
			public UseMyOption(IOptions<MyOption> option)
			{
				Option = option.Value;
			}
			public MyOption Option { get; set; }
		}

	    public class MyOption
	    {
			// string 可以
			public string Name { get; set; }
			// 集合绑定到object, 失败, 值为null
			//public object Family { get; set; }
		    // 这样写没问题, 但是绑定的是null
		    //public IEnumerable<object> Family { get; set; }
			// 复杂类型, 只要类型能够对上,绑定成功
			public IEnumerable<MyOption2> Family { get; set; }
			// 简单int, 可以
			public int Age { get; set; }
			// 不存在的字段(string)可以, 值为null
			public string NotHaveInConfiguration { get; set; }
			// 不存在的字段(复杂类型)可以, 值为null
		    public string NotHaveInConfigurationOfObject { get; set; }
		}

	    public class MyOption2
	    {
			public string Name { get; set; }
		}
    }
}
