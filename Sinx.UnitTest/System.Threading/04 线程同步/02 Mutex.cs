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
	public class _02_Mutex
	{
		/// <summary>
		/// Mutex是一种原始的同步方式, 其只对一个线程授予访问共享资源的独占访问
		/// </summary>
		[Fact]
		public void Synchronization_Mutex_AvoidRaceCondition()
		{
			const string mutexName = nameof(mutexName);
			using (var mutex = new Mutex(false, mutexName))
			{
				if (!mutex.WaitOne(TimeSpan.FromSeconds(2), false))
				{
					Console.WriteLine("第二个实例正在运行...");
				}
				else
				{
					Console.WriteLine();
				}
			}
		}
	}
}
