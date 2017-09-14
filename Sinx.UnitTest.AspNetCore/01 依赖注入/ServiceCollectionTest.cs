using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._01_“¿¿µ◊¢»Î
{
    public class ServiceCollectionTest
    {
        [Fact]
        public void ServiceCollection_ConstructServiceProvider()
        {
			// ServiceDescriptor ºØ∫œ
	        var collection = new ServiceCollection();
	        Assert.Empty(collection);
			var provider = collection.BuildServiceProvider();
	        var service = provider.GetService<object>();
	        Assert.Null(service);
        }
    }
}
