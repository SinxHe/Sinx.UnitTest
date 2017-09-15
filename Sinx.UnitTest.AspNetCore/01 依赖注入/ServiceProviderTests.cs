using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._01_依赖注入
{
    public class ServiceProviderTests
    {
		/// <summary>
		/// 使用一个ServiceProvider创建另一个ServiceProvider后, 他们之间是有父子关系的, 且不管多少代产生, 父子关系只有一级
		/// </summary>
		/// <remarks>
		/// sp0 -> sp1 -> sp2 -> sp3
		/// 父: sp0
		/// 子: sp1, sp2, sp3
		/// </remarks>
		[Fact]
	    public void ServiceProvider_CreateServiceProvider_UseAnotherServiceProvider()
		{
			var serviceProvider0 = new ServiceCollection().BuildServiceProvider();
			var serviceProvider1 = serviceProvider0
				.GetService<IServiceScopeFactory>()
				.CreateScope()
				.ServiceProvider;
			var root = serviceProvider1
				.GetType()
				.GetProperty("Root", BindingFlags.Instance|BindingFlags.NonPublic)
				.GetValue(serviceProvider1);
			// serviceProvider1的父亲是serviceProvider0
			Assert.Same(root, serviceProvider0);
			var serviceProvider2 = serviceProvider1
				.GetService<IServiceScopeFactory>()
				.CreateScope()
				.ServiceProvider;
			root = serviceProvider2
				.GetType()
				.GetProperty("Root", BindingFlags.Instance | BindingFlags.NonPublic)
				.GetValue(serviceProvider1);
			// 父子关系的层级只有一级
			Assert.Same(root, serviceProvider0);
		}
    }
}
