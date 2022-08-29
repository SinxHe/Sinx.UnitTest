using Microsoft.Extensions.Logging;
using Orleans.Grains;
using Orleans.Runtime;

namespace Orleans.Server
{
	public class HelloGrain : Grain, IHello
	{
		private readonly ILogger<HelloGrain> _logger;
		private readonly IList<IDisposable> _timers = new List<IDisposable>();
		public HelloGrain(ILogger<HelloGrain> logger)
		{
			_logger = logger;
			// dispose 即取消定时器, 在Grain失活或者Silo崩溃时定时器会自动取消
			// 回调会保证单线程执行
			var timer = RegisterTimer(_ =>
			{
				Console.WriteLine($"Grain: {this.GetPrimaryKeyLong()}, HashCode: {GetHashCode()}");
				return Task.CompletedTask;
			}, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
			_timers.Add(timer);
			// reminder 会进行持久化, 会持续触发直到被取消, 甚至在集群重启后仍然存在
			// 如果集群关闭, 错过了一个触发时间点, 则会跳过在下一个触发点进行触发
			// 如果在触发的时候Grain没有被激活, 则会激活他
			// reminder 是通过消息来触发的, 周期应该是分钟, 小时或天
			// 存储通过UseXXReminderServce来指定, 比如, UseAzureTableReminderService
			// 需要实现ReceiveReminder方法
			var reminder = RegisterOrUpdateReminder("HelloReminder", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
		}

		public Task<string> SayHello(string greeting)
		{
			_logger.LogInformation("SayHello message received: greeting = '{Greeting}'", greeting);
			return Task.FromResult($"You said: '{greeting}', I say: Hello!");
		}

		// 在Grain激活的时候调用
		// 如果读取持久化数据失败, 这个方法不会被调用, ReadStateAsync会收到异常, 发送给Grain的请求会收到BadProviderConfigException
		public override Task OnActivateAsync()
		{
			return base.OnActivateAsync();
		}

		// 在Grain挂起的时候调用, 不能保证一定会被调用, 所以保存状态的逻辑不能在这里实现
		public override Task OnDeactivateAsync()
		{
			return base.OnDeactivateAsync();
		}
	}
	
	public class ReminderImpl : IRemindable
	{
		public Task ReceiveReminder(string reminderName, TickStatus status)
		{
			Console.WriteLine($"Reminder: {reminderName}");
			return Task.CompletedTask;
		}
	}
}
