using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._01_依赖注入
{
    public class ServiceCollectionTest
    {
        [Fact]
        public void ServiceCollection_ConstructServiceProvider()
        {
			// ServiceDescriptor 集合
	        var collection = new ServiceCollection();
	        Assert.Empty(collection);
			var provider = collection.BuildServiceProvider();
	        var service = provider.GetService<object>();
	        Assert.Null(service);
        }
    }
}
