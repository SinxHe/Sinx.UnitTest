using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Threading
{
    public class ThreadPoolTest
    {
		[Fact]
	    public void ThreadPool_Enqueue()
		{
			var ar = new[] {0};
			// 创建工作项
			WaitCallback workItem = obj => ar[0] = 1;
			// 加入线程池队列 queue v. 排列
			ThreadPool.QueueUserWorkItem(workItem);
			Thread.Sleep(500);
			Assert.Equal(ar[0], 1);
		}
    }
}
