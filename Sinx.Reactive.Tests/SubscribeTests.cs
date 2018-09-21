using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
// ReSharper disable MethodSupportsCancellation

namespace Sinx.Reactive.Tests
{
	public class SubscribeTests
	{
		/// <summary>
		/// Subscribe正常执行
		/// </summary>
		/// <remarks>
		/// - Subscribe只是将Observer注册进Observable然后触发订阅
		///	- 执行完成, 自动触发Observer.OnComplete
		/// </remarks>
		[Fact]
		public async Task Reactive_Suscribe()
		{
			async Task CreateAndConsumeAction(IObserver<int> o)
			{
				for (int i = 0; i < 3; i++)
				{
					await Task.Delay(TimeSpan.FromSeconds(1));
					$"Created {i}".Dump();
					o.OnNext(i);
					$"Consumed {i}".Dump();
					"----------------".Dump();
				}
			}

			var observable = Observable.Create((Func<IObserver<int>, Task>)CreateAndConsumeAction);
			// Suscribe 用于生成Observer, 然后传递给Observable, 然后启动数据生产的lambda表达式
			var tokenSource = new CancellationTokenSource();
			observable
				.Subscribe(e =>
				{
					$"Consuming {e}".Dump();
				}, ex => $"OnException Trigged {ex}".Dump(), () =>
				{
					"Completed".Dump();
					tokenSource.Cancel();
				});
			"★★★★★Finished★★★★★".Dump();
			await Task.Delay(TimeSpan.FromSeconds(1000), tokenSource.Token).ContinueWith(_ => { });
		}

		/// <summary>
		/// Subscribe生产数据出现异常
		/// </summary>
		/// <remarks>
		/// - 触发Observer.OnException
		/// - 生产数据停止
		/// </remarks>
		[Fact]
		public async Task Reactive_Suscribe_DataCreateException()
		{
			async Task CreateAndConsumeAction(IObserver<int> o)
			{
				for (int i = 0; i < 3;)
				{
					await Task.Delay(TimeSpan.FromSeconds(1));
					$"Created {i}".Dump();
					o.OnNext(i);
					throw new Exception("Exception On Data Create");
				}
			}

			var observable = Observable.Create((Func<IObserver<int>, Task>)CreateAndConsumeAction);
			// Suscribe 用于生成Observer, 然后传递给Observable, 然后启动数据生产的lambda表达式
			var tokenSource = new CancellationTokenSource();
			observable
				.Subscribe(e =>
				{
					$"Consuming {e}".Dump();
				}, ex =>
				{
					$"OnException Trigged {ex}".Dump();
					tokenSource.Cancel();
				}, () =>
				{
					"Completed".Dump();
					tokenSource.Cancel();
				});
			"★★★★★Finished★★★★★".Dump();
			await Task.Delay(TimeSpan.FromSeconds(1000), tokenSource.Token).ContinueWith(_ => { });
		}

		/// <summary>
		/// Subscribe消费数据出现异常
		/// </summary>
		/// <remarks>
		/// - 因为Observer.OnNext中出现了异常以后Observer就会直接停止订阅(Observer.IsStop为true), 所以数据生产端即使调用了Observer.OnError也不会在导致OnError的执行了
		/// - 导致生产数据停止
		/// - 所以在数据消费中出现异常是不好的做法
		/// </remarks>
		[Fact]
		public async Task Reactive_Suscribe_DataConsumingException()
		{
			var tokenSource = new CancellationTokenSource();
			async Task CreateAndConsumeAction(IObserver<int> o)
			{
				for (int i = 0; i < 3; i++)
				{
					await Task.Delay(TimeSpan.FromSeconds(1));
					$"Created {i}".Dump();
					try
					{            
						o.OnNext(i);
					}
					catch (Exception ex)
					{
						$"Exception In OnNext {ex}".Dump();
						// 手动触发Observer.OnError, 一样没用, 因为一旦Subscribe中出现了异常, 则Observer的订阅就停止了, 所以这里的OnError根本出不去
						o.OnError(ex);
						tokenSource.Cancel();
						throw;
					}

					$"Consumed {i}".Dump();
					"----------------".Dump();
				}
			}

			var observable = Observable.Create((Func<IObserver<int>, Task>)CreateAndConsumeAction);
			// Suscribe 用于生成Observer, 然后传递给Observable, 然后启动数据生产的lambda表达式
			observable
				.Subscribe(e =>
				{
					$"Consuming {e}".Dump();
					throw new Exception("Exception In Observer.OnNext");
				}, ex =>
				{
					$"OnException Trigged {ex}".Dump();
					tokenSource.Cancel();
				}, () =>
				{
					"Completed".Dump();
					tokenSource.Cancel();
				});
			"★★★★★Finished★★★★★".Dump();
			await Task.Delay(TimeSpan.FromSeconds(1000), tokenSource.Token).ContinueWith(_ => { });
		}
	}
}
