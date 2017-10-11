using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
    public class _06IOptionsSnapshotTests
    {
		[Fact]
	    public void IOptionsSnapshot_NormalBind()
	    {
		    var path = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "appsettings.json");
		    var conf = new ConfigurationBuilder()
			    .AddJsonFile(path, false, true)
				.Build();
		    var sp = new ServiceCollection()
				.AddOptions()
				.Configure<MyOptions>(conf)
				.AddScoped<UseMyOptions>()
				.BuildServiceProvider();
		    var options = sp.GetService<UseMyOptions>();
		    Assert.Equal(options.Options.Name, "SinxHe");
		    var orignalContent = File.ReadAllText(path);
		    File.WriteAllText(path, orignalContent.Replace("SinxHe", "DaXiong"));
		    Task.Delay(TimeSpan.FromSeconds(12)).Wait();
			
		    var t = Task.Run(() =>
		    {
			    options = sp.GetRequiredService<UseMyOptions>();
				//Assert.Equal(options.Options.Name, "DaXiong");
			});
		    t.ConfigureAwait(false);
		    t.Wait();
			
			File.WriteAllText(path, orignalContent);
	    }

	    public class UseMyOptions
	    {
			public MyOptions Options { get; set; }
			public UseMyOptions(IOptionsSnapshot<MyOptions> optionsAccessor)
			{
				Options = optionsAccessor.Value;
			}
	    }

	    public class MyOptions
	    {
			public string Name { get; set; }
			public int Age { get; set; }
		}
    }
}
