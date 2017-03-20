using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Threading
{
	/// <summary>
	/// 执行上下文类用户干预执行上下文从初始线程"流"向辅助线程
	/// </summary>
	public class ExecutionContextTests
	{
		[Fact]
		public void ExecutionContext_SuppressFlow()
		{
			// local func
			string[] ar = new string[1];
			AutoResetEvent are = new AutoResetEvent(false);

			Action<object> workItem = state =>
			{
				are.Set();
				ar[0] = (string)CallContext.LogicalGetData("name");
			};

			// 将一些数据放到初始线程的逻辑上下文中(逻辑调用上下文是执行上下文中的一个数据结构)
			CallContext.LogicalSetData("name", "sinx");

			ThreadPool.QueueUserWorkItem(state => workItem(state));
			are.WaitOne();
			Assert.Equal(ar[0], "sinx");

			ar[0] = null;
			// 阻止执行上下文流动, 此时辅助线程选用上次和他关联的执行上下文
			ExecutionContext.SuppressFlow();
			are.Reset();
			ThreadPool.QueueUserWorkItem(state => workItem(state));
			are.WaitOne();
			Assert.Null(ar[0]);

			// 恢复 !这个初始线程! 的执行上下文流动, 以免将来的辅助线程获取不到执行上下文
			ExecutionContext.RestoreFlow();
		}
	}
}
