using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Sinx.UnitTest.Base
{
	public class GCNotificationTest
	{
		//[Fact]TODO 这玩意怎么用
		public void GC_GCNotification()
		{
			var obj = new object();
			obj = null;
			GCNotification.GCDone += GCNotification_GCDone;
			GC.Collect();
			Thread.Sleep(TimeSpan.FromSeconds(1));
		}

		private void GCNotification_GCDone(int obj)
		{
		
		}
	}

	public static class GCNotification
	{
		private static Action<int> s_gcDone;	// 事件的字段
		public static event Action<int> GCDone
		{
			add
			{
				// 如果之前没有等级的委托, 就开始报告通知
				if (s_gcDone == null)
				{
					new GenObj(0);
					new GenObj(2);
				}
				s_gcDone += value;
			}
			remove => s_gcDone -= value;
		}

		private sealed class GenObj
		{
			private int m_generation;

			public GenObj(int generation)
			{
				m_generation = generation;
			}

			~GenObj()
			{
				// 这是Finalize方法
				// 如果这个对象在我们希望的(或更高的)代中
				// 就通知委托一次GC刚刚完成
				if (GC.GetGeneration(this) < m_generation) return;
				var temp = Volatile.Read(ref s_gcDone);
				temp?.Invoke(m_generation);
				// 如果至少还有一个已登记的委托, 而且AppDomain并非正在卸载
				// 而且进程并非正在关闭, 就继续报告
				if (s_gcDone != null
					&& !AppDomain.CurrentDomain.IsFinalizingForUnload()
					&& !Environment.HasShutdownStarted)
				{
					// 对于0代, 创建一个新对象, 对于2代, 复活对象
					// 使2代在下次回收时, GC会再次调用Finalize
					if (m_generation == 0)
					{
						new GenObj(0);
					}
					else
					{
						GC.ReRegisterForFinalize(this);
					}
				}
				else
				{
					// 放过对象, 让其回收
				}
			}
		}
	}
}
