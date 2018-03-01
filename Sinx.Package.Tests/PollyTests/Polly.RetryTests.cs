using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Xunit;

namespace Sinx.Package.Tests.PollyTests
{
	public class PollyTests
	{
		private readonly ILogger _logger;
		public PollyTests()
		{
			var sp = new ServiceCollection()
				.AddLogging(b =>
				{
					b.AddConsole().AddDebug().AddEventSourceLogger().AddTraceSource("switchName");
					b.AddFilter((_, __) => true);
				})
				.BuildServiceProvider();
			_logger = sp.GetRequiredService<ILogger<PollyTests>>();
		}

		[Fact]
		public void Polly_Retry()
		{
			// Retry Once
			Policy
				.Handle<Exception>()
				.Retry();

			// Retry Mutiple Times
			Policy
				.Handle<ArgumentOutOfRangeException>()
				.Retry(3);

			// Exception Handler When Exception
			Policy
				.Handle<Exception>()
				.Retry(3, (ex, retryCount, context) =>
				{
					_logger.LogInformation(retryCount.ToString());
					_logger.LogInformation(JsonConvert.SerializeObject(context));
				});

			// Wait And Retry
			Policy
				.Handle<Exception>()
				.WaitAndRetry(Enumerable.Range(1, 3).Select(i => TimeSpan.FromSeconds(i)));

			// Wait And Retry, When Retry, Calling An Action
			Policy
				.Handle<Exception>()
				.WaitAndRetry(
					Enumerable.Range(1, 3).Select(i => TimeSpan.FromSeconds(i)),
					(ex, timeSpan) => { _logger.LogInformation(ex.ToString()); });

			// 使用指定的延时策略
			// In this case will wait for
			//  2 ^ 1 = 2 seconds then
			//  2 ^ 2 = 4 seconds then
			//  2 ^ 3 = 8 seconds then
			//  2 ^ 4 = 16 seconds then
			//  2 ^ 5 = 32 seconds
			Policy
				.Handle<Exception>()
				.WaitAndRetry(
					5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
		}

		[Fact]
		public void Polly_UsePolicy()
		{
			var policy = Policy
				.Handle<Exception>()
				.WaitAndRetry(3,
					retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)),
					(ex, ts, ctx) => { _logger.LogWarning(ts.ToString()); });

			policy.Execute(() => throw new Exception());
		}

		[Fact]
		public void Polly_Context()
		{
			var policy = Policy
				.Handle<Exception>()
				.WaitAndRetry(3, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)),
						(ex, ts, ctx) => { _logger.LogInformation(JsonConvert.SerializeObject(ctx)); });
			policy.Execute(() => throw new Exception(), new Dictionary<string, object>
			{
				["methodName"] = "Polly_Context"
			});
		}
	}
}
