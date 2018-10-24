using System.Threading;
using Xunit;

namespace Sinx.UnitTest.System.Threading._01_线程基础操作
{
	public class 给线程传递参数
	{
		[Fact]
		public void Thread_DeliverParams_Lambda()
		{
			int n = 1;
			var thread = new Thread(() => n++);
			// Lambda表达式定义了一个不属于任何类的方法
			// 这里Lambda表达式在使用任何局部变量的时候, 编译器会帮我们把局部变量封装到一个匿名类的属性上
			// 而这个局部变量是不存在的, 这里的局部变量和Lambda中使用的变量都是匿名类的属性
			thread.Start();
			thread.Join();
			Assert.Equal(2, n);
		}

		[Fact]
		public void Thread_DeliverParams_ClassDeliver()
		{
			var deliverClass = new DeliverClass { N = 1};
			var thread = new Thread(deliverClass.PlusPlus);
			// 这里的实现方式跟 int n = 1; var thread = new Thread(() => n++); 一模一样, 只不过这里的匿名类是自己创建的非匿名类而已
			// 注意: 如果这里的调用改成了var thread = new Thread(() => deliverClass.PlusPlus()); 那么还是会再创建一个匿名类, 将匿名类的Lambda实现方法赋值给Thread(Action)
			thread.Start();
			thread.Join();
			Assert.Equal(2, deliverClass.N);
		}

		[Fact]
		public void Thread_DeliverParams_StartMethod()
		{
			var thread = new Thread(o => Assert.Equal(1, o));
			thread.Start(1);
			// 取消装箱的结果是无法改变的
			// 使用Start传值的方式对于结果的传出很麻烦
		}

		private class DeliverClass
		{
			public int N { get; set; }

			public void PlusPlus()
			{
				N++;
			}
		}
	}
}
