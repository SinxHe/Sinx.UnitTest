using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.垃圾回收
{
	public class _01_跟一直在方法内导致的问题
	{
		[Fact]
		public void GC_RootAlwaysInMethod()
		{
			var i = 0;
			var timer = new Timer(o =>
			{
				i++;
				GC.Collect();
			}, null, 0, 100);
			Thread.Sleep(1000);
			Assert.Equal(1, i);
			// 回收开始时
		}
	}
}
