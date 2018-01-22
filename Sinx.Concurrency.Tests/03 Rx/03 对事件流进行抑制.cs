using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Concurrency.Tests._03_Rx
{
	public class _03_对事件流进行抑制
	{
		/// <summary>
		/// 根据事件的数据进行过滤
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task Observable_Where()
		{
			var observable = Observable.Interval(TimeSpan.FromMilliseconds(500))
				.Where(x => x % 2 == 0);
			observable.Subscribe(Console.WriteLine);
			await observable;
		}

		/// <summary>
		/// 超时: 指定时间没有数据, TimeoutException的OnError, 否则, 重置
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task Observable_Timeout()
		{
			var observable = Observable
				.Interval(TimeSpan.FromMilliseconds(50))
				//.Interval(TimeSpan.FromMilliseconds(500))
				.Timeout(TimeSpan.FromMilliseconds(200));
			observable.Subscribe(
				Console.WriteLine, 
				ex => Console.WriteLine(ex));
			await observable;
		}

		/// <summary>
		/// 超时: 超过设置的时间段没有数据的时候就会将最后一个事件发送出去
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task Observable_Throttle()
		{
			var observable = Observable
				//.Interval(TimeSpan.FromMilliseconds(500))
				.Interval(TimeSpan.FromMilliseconds(2000))
				.Throttle(TimeSpan.FromMilliseconds(1000));
			observable.Subscribe(Console.WriteLine);
			await observable;
		}

		/// <summary>
		/// 超时: 每个时间段结束以后将事件发送出去
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task Observable_Sample()
		{
			var observable = Observable.Interval(TimeSpan.FromMilliseconds(50))
				.Sample(TimeSpan.FromMilliseconds(1000));
			observable.Subscribe(Console.WriteLine);
			await observable;
		}
	}
}
