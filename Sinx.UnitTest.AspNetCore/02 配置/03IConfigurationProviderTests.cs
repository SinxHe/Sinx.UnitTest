using Microsoft.Extensions.Configuration.Json;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
    public class _03IConfigurationProviderTests
    {
		[Fact]
	    public void IConfigurationProvider_()
		{
			var provider = new JsonConfigurationProvider(new JsonConfigurationSource
			{
				Path = "settings.json"
			});
			provider.TryGet("Name", out var name);
			Assert.Equal("Name", name);
		}
    }
}
