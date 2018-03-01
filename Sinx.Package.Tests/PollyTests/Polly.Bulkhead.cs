using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Xunit;

namespace Sinx.Package.Tests.PollyTests
{
    public partial class PollyTests
    {
		[Fact]
	    public async Task Policy_Bulkhead()
		{
			var policy = Policy.BulkheadAsync(
				3, 1,
				ctx =>
				{
					_logger.LogInformation("------------------------------");
					_logger.LogInformation(JsonConvert.SerializeObject(ctx));
					return Task.CompletedTask;
				});

			var taskList = Enumerable.Range(0, 100)
				.Select(i =>
				{
					return policy.ExecuteAsync(async () =>
					{
						await Task.Delay(TimeSpan.FromMilliseconds(50));
						_logger.LogInformation($"ThreadId: {Thread.CurrentThread.ManagedThreadId} Id: {i}");
					});
				});
			await Task.WhenAll(taskList);
		}
    }
}
