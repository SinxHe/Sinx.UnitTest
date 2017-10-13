using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.Base
{
	public class LazyTest
	{
		[Fact]
		public void Lazy_NormalUsage()
		{
			// 使用默认构造函数进行初始化, 如果没有默认构造函数, 抛出异常
			var ctorLazy = new Lazy<object>();
			Assert.NotNull(ctorLazy.Value);

			var notLazy = DateTime.Now;
			var lazy = new Lazy<DateTime>(() => DateTime.Now);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			var ts = lazy.Value - notLazy;
			Assert.True(ts.TotalSeconds > 2);
		}

		[Fact]
		public void ThreadLocal_NormalUsage()
		{
			var time = DateTime.Now;
			var threadLocal = new ThreadLocal<DateTime>(() => DateTime.Now);
			Assert.True((threadLocal.Value - time).TotalSeconds < 1);
			Task.Delay(TimeSpan.FromSeconds(2)).Wait();
			Task.Run(() =>
			{
				// 各个线程保存自己的延迟值
				Assert.True((threadLocal.Value - time).TotalSeconds > 2);
			});
			Assert.True((threadLocal.Value - time).TotalSeconds < 1);
		}

		[Fact]
		public void ThreadStaticAttribute_NormalUsage()
		{
			// 1. ThreadStatic 仅在 Static 字段(不是属性)上有效
			// 2. 字段在初始化线程是初始化的值, 在其他线程是默认值
			var student = new Student();
			Assert.True(student.ThreadLocalData == 1);
			Assert.Equal(student.Data, 1);
			Task.Run(() =>
			{
				Assert.Equal(student.ThreadLocalData, 0);
				Assert.Equal(student.Data, 1);
			});
			Assert.True(student.ThreadLocalData == 1);
			Assert.Equal(student.Data, 1);
		}

		[Fact]
		public void LazyInitializer_NormalUsage()
		{
			// 用起来麻烦
		}

		public class Student
		{
			[ThreadStatic] private static int _threadLocalData = 1;
			private static int _data = 1;
			public  int ThreadLocalData => _threadLocalData;
			public  int Data => _data;
		}
	}
}
