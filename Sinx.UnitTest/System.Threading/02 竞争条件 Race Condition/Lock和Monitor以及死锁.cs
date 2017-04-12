using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Threading._02_竞争条件_Race_Condition
{
	public class Lock和Monitor以及死锁
	{
		[Fact]
		public void RaceCondition_Lock()
		{
			int n = 0;

			var thread0 = new Thread(Operate);
			var thread1 = new Thread(Operate);
			var thread2 = new Thread(Operate);
			thread0.Start();
			thread1.Start();
			thread2.Start();
			thread0.Join();
			thread1.Join();
			thread2.Join();
			Assert.NotEqual(n, 0);

			n = 0;
			var obj = new object(); // 互斥锁
			thread0 = new Thread(OperateWithLock);
			thread1 = new Thread(OperateWithLock);
			thread2 = new Thread(OperateWithLock);
			thread0.Start();
			thread1.Start();
			thread2.Start();
			thread0.Join();
			thread1.Join();
			thread2.Join();
			Assert.Equal(n, 0);

			void Operate()
			{
				for (var i = 0; i < 100000; i++)
				{
					n++;
					n--;
				}
			}

			void OperateWithLock()
			{
				lock (obj)
				{
					Operate();
				}
			}
		}

		[Fact]
		public void RaceCondition_Monitor()
		{
			var ar = new[] { 0 };
			var exeAr = new object();
			Thread thd1 = new Thread(() =>
			{
				Monitor.Enter(exeAr);
				try
				{
					Thread.Sleep(500);
					ar[0] = 1;
				}
				finally     // 其实在 lock 中帮忙做了这个
				{
					Monitor.Exit(exeAr);
				}
			});
			Thread thd2 = new Thread(() =>
			{
				Monitor.Enter(exeAr);
				try
				{
					ar[0] = 2;
				}
				finally     // 其实在 lock 中帮忙做了这个
				{
					Monitor.Exit(exeAr);
				}
			});
			thd1.Start();
			Thread.Sleep(100);
			thd2.Start();
			Thread.Sleep(100);  // 恢复线程2的时候要保证当前线程不会锁住exeAr
			Monitor.Enter(exeAr);
			try
			{
				Assert.Equal(2, ar[0]);
			}
			finally     // 其实在 lock 中帮忙做了这个
			{
				Monitor.Exit(exeAr);
			}
		}

		[Fact]
		public void RaceCondition_Deadlock()
		{
			var invokeThread = Thread.CurrentThread;
			var isDeadlock = false;
			var timer = new Timer(o =>
				{
					Assert.True(isDeadlock);
					invokeThread.Abort();	// 防止无限期执行 
				},
				null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
			var o1 = new object();
			var o2 = new object();
			// 锁定o1
			var thread = new Thread(() => LockObj1(o1, o2));
			thread.Start();
			Thread.Sleep(100);
			lock (o2)   // 锁定o2, 使得LockObj1()无法锁定o2, 一直等待
			{
				isDeadlock = true;
				lock (o1)	
				{
					// LockObj1()等待调用线程解锁o2才能解锁o1
					// 调用线程等待LockObj1()解锁o1才能解锁o2
					isDeadlock = false;
				}
			}
		}

		[Fact]
		public void RaceCondition_MonitorTryEnter_AvoidDeadLock()
		{
			var o1 = new object();
			var o2 = new object();
			// 锁定o1
			var thread = new Thread(() => LockObj1(o1, o2));
			thread.Start();
			Thread.Sleep(100);
			lock (o2)   // 锁定o2, 使得LockObj1()无法锁定o2, 一直等待
			{
				// 使用Monitor.TryEnter允许一秒钟等待的加锁, 避免线程死锁
				var b = Monitor.TryEnter(o1, TimeSpan.FromSeconds(1));
				Assert.False(b);    // 尝试锁定o1失败
			}
		}

		private static void LockObj1(object obj1, object obj2)
		{
			lock (obj1)
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(500));
				lock (obj2) { }
			}
		}
	}
}
