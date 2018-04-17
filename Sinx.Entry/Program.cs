using System;
using System.Threading.Tasks;
using Dora.DynamicProxy;
using Dora.Interception;
using Microsoft.Extensions.DependencyInjection;

namespace Sinx.Entry
{
	public static class Program
	{
		static void Main(string[] args)
		{
			var demo = new ServiceCollection()
					.AddSingleton<IDemo, Demo>()
					.BuildInterceptableServiceProvider()
					.GetRequiredService<IDemo>();
			demo.InvokeAsync();
			Console.WriteLine("Continue...");
			Console.Read();
		}
		public class FoobarInterceptor
		{
			private InterceptDelegate _next;

			public FoobarInterceptor(InterceptDelegate next)
			{
				_next = next;
			}
			public async Task InvokeAsync(InvocationContext context)
			{
				Console.WriteLine("Interception task starts.");
				await Task.Delay(1000);
				Console.WriteLine("Interception task completes.");
				await _next(context);
			}
		}

		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
		public class FoobarAttribute : InterceptorAttribute
		{
			public override void Use(IInterceptorChainBuilder builder)
			{
				builder.Use<FoobarInterceptor>(this.Order);
			}
		}

		public interface IDemo
		{
			Task InvokeAsync();
		}

		public class Demo : IDemo
		{
			[Foobar]
			public Task InvokeAsync()
			{
				Console.WriteLine("Target method is invoked.");
				return Task.CompletedTask;
			}
		}
	}
}
