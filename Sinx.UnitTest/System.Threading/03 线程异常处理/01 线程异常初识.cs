using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Sinx.UnitTest.System.Threading._03_线程异常处理
{
	public class _01_线程异常初识
	{
		/// <summary>
		/// 多线程在回调抛出的异常并不能被调用方法捕获, 事实上, 调用方法可能已经执行完毕
		/// 如果调用线程在等待回调执行完毕, 那么在回调方法抛出异常之后, 目标线程变成了Stopped状态,
		/// 调用线程正常执行, 好像没有发生异常一样
		/// </summary>
		[Fact]
		public void Thread_Exception_ThrowAExceptionInCallBack()
		{
			var t = new Thread(ThrowAndCatch);
			t.Start();
			t.Join();

			var isInCatchBlock = false;
			try
			{
				t = new Thread(Throw);
				t.Start();
				t.Join();
			}
			catch (Exception)
			{
				isInCatchBlock = true;
			}
			Assert.False(isInCatchBlock);

			void Throw()
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
				throw new Exception("Boom!");
			}

			void ThrowAndCatch()
			{
				var isInCatch = false;
				try
				{
					Throw();
				}
				catch (Exception)
				{
					// 一般来说不要在线程中抛出异常而是在线程中使用try..catch块
					// 当然, 在异步中对此的处理是将异常信息包装在了状态机中进行返回
					isInCatch = true;
				}
				finally
				{
					Assert.True(isInCatch);
				}
			}
		}
	}
}
