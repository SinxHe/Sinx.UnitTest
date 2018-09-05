using System;
using System.Diagnostics;
using Xunit;

namespace Sinx.Reactive.Tests
{
	public class IObservableTests
	{
		/// <summary>
		/// 使用Subscribe产生数据, 然后在生成数据以后通知Observer进行处理
		/// </summary>
		[Fact]
		public void IObservable_Subscribe()
		{
			// Arrange
			var observable = new MyObservable();
			var observer = new MyObserver();

			// Act
			observable.Subscribe(observer);

			// Assert
		}

		private class MyObservable : IObservable<int>
		{
			public IDisposable Subscribe(IObserver<int> observer)
			{
				observer.OnNext(1);
				observer.OnCompleted();
				return default;
			}
		}

		private class MyObserver : IObserver<int>
		{
			public void OnCompleted()
			{
				Debug.WriteLine("Finished Event Observed");
			}

			public void OnError(Exception error)
			{
				throw new NotImplementedException();
			}

			public void OnNext(int value)
			{
				Debug.WriteLine(value + " be observed");
			}
		}
	}
}
