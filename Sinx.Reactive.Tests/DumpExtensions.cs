using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sinx.Reactive.Tests
{
	public static class DumpExtensions
	{
		private static readonly ILogger _logger;

		static DumpExtensions()
		{
			_logger = new ServiceCollection()
				.AddLogging(b => b.AddDebug())
				.BuildServiceProvider()
				.GetRequiredService<ILoggerFactory>()
				.CreateLogger("Observable");
		}

		public static T Dump<T>(this T ins)
		{
			_logger.LogInformation(ins?.ToString() ?? "[null]");
			return ins;
		}
	}
}
