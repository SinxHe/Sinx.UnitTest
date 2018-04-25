using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Xunit;

namespace Sinx.Quartz.Tests
{
	public class QuartzTests
	{
		[Fact]
		public async Task Quartz_StartAsync()
		{
			var factory = new StdSchedulerFactory();
			var scheduler = await factory.GetScheduler();
			await scheduler.Start();
			var job = JobBuilder.Create<HelloJob>()
				// 定义在调度器中的Key值
				.WithIdentity("job1", "group1")
				.Build();

			var trigger = TriggerBuilder.Create()
				.WithIdentity("trigger1", "group1")
				.StartNow()
				.WithSimpleSchedule(x => x
					.WithIntervalInSeconds(1)
					.RepeatForever())
				.Build();

			await scheduler.ScheduleJob(job, trigger);

			await Task.Delay(TimeSpan.FromSeconds(6));
			await scheduler.Shutdown();
		}

		[Fact]
		public async Task Quartz_Job_Injection()
		{
			var services = new ServiceCollection()
				.AddLogging(b => b.AddConsole())
				.BuildServiceProvider();
			var data = new JobDataMap {new KeyValuePair<string, object>(nameof(ServiceProvider), services)};

			var factory = new StdSchedulerFactory();
			var scheduler = await factory.GetScheduler();
			await scheduler.Start();
			var job = JobBuilder.Create<HelloJob>()
				.WithIdentity("Injection")
				.UsingJobData(data)
				.Build();
			var trigger = TriggerBuilder.Create()
				.WithIdentity("Injection")
				.StartNow()
				.WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever())
				.Build();
			await scheduler.ScheduleJob(job, trigger);
			await Task.Delay(TimeSpan.FromSeconds(6));
			await scheduler.Shutdown();
		}
	}
}
