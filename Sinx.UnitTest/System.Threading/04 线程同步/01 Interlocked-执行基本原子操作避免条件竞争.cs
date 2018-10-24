using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Sinx.UnitTest.System.Threading._04_线程同步
{
	public class _01_执行基本原子操作避免条件竞争
	{
		[Fact]
		public void AtomicOperate_AvoidRaceCondition()
		{
			// -----------------------------------
			// 条件竞争
			var count = 0;
			void Operate()
			{
				for (var i = 0; i < 100000; i++)
				{
					count++;
					count--;
				}
			}
			var thread1 = new Thread(Operate);
			var thread2 = new Thread(Operate);
			var thread3 = new Thread(Operate);
			var threads = new List<Thread> { thread1, thread2, thread3};
			threads.ForEach(t => t.Start());
			threads.ForEach(t => t.Join());
			Assert.NotEqual(0, count);
			// -----------------------------------
			// 原子操作 - 线程同步 - 避免条件竞争
			var number = 0;

			void AtomicOperate()
			{
				for (var i = 0; i < 100000; i++)
				{
					Interlocked.Increment(ref number);	// 递增原子操作
					Interlocked.Decrement(ref number);	// 递减原子操作  
				}
			}

			var t1 = new Thread(AtomicOperate);
			var t2 = new Thread(AtomicOperate);
			var t3 = new Thread(AtomicOperate);
			var ts = new List<Thread> { t1, t2, t3};
			ts.ForEach(t => t.Start());
			ts.ForEach(t => t.Join());
			Assert.Equal(0, number);
		}
	}
}
