using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sinx.Concurrency.Tests._02_Task任务
{
	public class CreateTaskTests
	{
		public async Task Task_Create()
		{
			// --- 计算任务 ---
			// 常用
			await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(1)).Wait());
			// 按照计划执行(可以传入Scheduler)
			await new TaskFactory().StartNew(() => Task.Delay(TimeSpan.FromSeconds(1)).Wait());

			// --- 事件任务 ---

			// 事件任务
			// TaskCompletionSource<> // TODO 还不会用

			// IO任务大部分采用TaskCompletionSource<>
		}
	}
}
