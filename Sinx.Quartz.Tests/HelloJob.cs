using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Sinx.Quartz.Tests
{
    internal class HelloJob : IJob
    {
		public Task Execute(IJobExecutionContext context)
	    {
		    Console.WriteLine("Hello");
		    var services = context.MergedJobDataMap.Get(nameof(ServiceProvider)) as IServiceProvider;
		    var logger = services?.GetRequiredService<ILoggerFactory>().CreateLogger<HelloJob>();
		    logger?.LogInformation("Hello");
			return Task.CompletedTask;
	    }
    }
}
