using System;
using System.Threading.Tasks;
using Dora.DynamicProxy;
using Dora.Interception;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Sinx.AOP.Tests
{
	public class Dora_Interception_Tests
	{
		private readonly IServiceProvider _sp;
		public Dora_Interception_Tests()
		{
			var sp = new ServiceCollection()
				.AddSingleton<IFoo, Foo>()
				.BuildInterceptableServiceProvider();
			_sp = sp;
		}

		[Fact]
		public async Task Dora_Interception_Start()
		{
			var foo = _sp.GetRequiredService<IFoo>();
			await foo.GetAsync();
			var dt = DateTimeOffset.Parse(Environment.GetEnvironmentVariable("Dora_Interception_Tests") ?? DateTimeOffset.Now.AddDays(-1).ToString());
			Assert.True(DateTimeOffset.Now - dt < TimeSpan.FromSeconds(60));
		}

		public interface IFoo
		{

			Task GetAsync();
		}

		public class Foo : IFoo
		{
			[Decorate]
			public Task GetAsync()
			{
				return Task.CompletedTask;
			}
		}
		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
		public class DecorateAttribute : InterceptorAttribute
		{
			public override void Use(IInterceptorChainBuilder builder)
			{
				builder.Use<DecorateInterceptor>(Order);
			}
		}

		public class DecorateInterceptor
		{
			private InterceptDelegate _next;
			public DecorateInterceptor(InterceptDelegate next)
			{
				_next = next;
			}

			public async Task InvokeAsync(InvocationContext context)
			{
				Environment.SetEnvironmentVariable("Dora_Interception_Tests", DateTimeOffset.Now.ToString());
				await Task.Delay(TimeSpan.FromSeconds(1));
				await _next(context);
			}
		}
	}
}
