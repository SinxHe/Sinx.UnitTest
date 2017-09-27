using System.Collections;
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
				.GetProperty("Root", BindingFlags.Instance | BindingFlags.NonPublic)
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

		[Fact]
		public void ServiceProvider_ServiceLifeTime()
		{
			var root = new ServiceCollection()
				.AddTransient<IA, A>()  // 每次都创建
				.AddScoped<IB, B>()     // 在作用域内使用单例
				.AddSingleton<IC, C>()  // 一直单例
				.BuildServiceProvider();
			var child0 = root
				.GetService<IServiceScopeFactory>()
				.CreateScope()
				.ServiceProvider;
			var child1 = root
				.GetService<IServiceScopeFactory>()
				.CreateScope()
				.ServiceProvider;
			// Transient: 每次都创建的, ServiceProvider创建的实例不一样
			Assert.NotSame(root.GetService<IA>(), root.GetService<IA>());
			// Scope: 同一个ServiceProvider创建的一样
			Assert.Same(child0.GetService<IB>(), child0.GetService<IB>());
			// Scope: 不同的ServiceProvider创建的不一样
			Assert.NotSame(child0.GetService<IB>(), child1.GetService<IB>());
			// Singleton: 同根的创建的都一样
			Assert.Same(child0.GetService<IC>(), child1.GetService<IC>());
		}

		[Fact]
		public void ServiceProvider_GetRequiredService_ThrowExceptionIfNotExists()
		{
			var sp = new ServiceCollection().BuildServiceProvider();
			Assert.Throws<System.InvalidOperationException>(() => sp.GetRequiredService<IEnumerable>());
			Assert.Null(sp.GetService<IEnumerable>());

			var services = new ServiceCollection();
			services.AddSingleton<IEnumerable>(new[] { 1, 2 });
			var enums = services.BuildServiceProvider().GetService<IEnumerable>();
			Assert.Equal(new []{1,2}, enums);
		}
	}

	public interface IA
	{
	}

	public interface IB
	{
	}

	public interface IC
	{
	}

	public class A : IA
	{
	}

	public class B : IB
	{
	}

	public class C : IC
	{
	}
}
