using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Sinx.DependencyInjection.Tests.MyDIContainer
{
	internal class BaseService : IDisposable, IService
	{
		private readonly ILogger _logger;

		public BaseService()
		{
			_logger = new DebugLoggerProvider().CreateLogger(GetType().Name);
			_logger.LogInformation("Created");
		}

		public void Dispose()
		{
			_logger.LogInformation("Disposed");
		}
	}

	internal interface IService
	{
	}

	internal class MyService : BaseService
	{
	}
}
