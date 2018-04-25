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
					.AddSingleton<Demo>()
					.BuildInterceptableServiceProvider()
					.GetRequiredService<Demo>();
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

		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
		public class FoobarAttribute : InterceptorAttribute
		{
			public override void Use(IInterceptorChainBuilder builder)
			{
				builder.Use<FoobarInterceptor>(Order);
			}
		}

		public interface IDemo
		{
			//[Foobar]
			Task InvokeAsync();
		}

		public class Demo : IDemo
		{
			[Foobar]
			public virtual Task InvokeAsync()
			{
				Console.WriteLine("Target method is invoked.");
				return Task.CompletedTask;
			}
		}
	}
}
