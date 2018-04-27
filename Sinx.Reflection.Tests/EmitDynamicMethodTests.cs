using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace Sinx.Reflection.Tests
{
	public class EmitDynamicMethodTests
	{
		private long test = 42;

		/// <summary>
		/// 声明用于执行方法的委托类型
		/// </summary>
		private delegate TReturn OneParameter<out TReturn, in TParameter>(TParameter p);

		/// <summary>
		/// 定义和执行绑定到模块的动态方法
		/// </summary>
		[Fact]
		public void EmitDynamicMethod_BindToModule()
		{
			// 创建动态方法
			Type[] methodArgs = { typeof(int) };
			var squareIt = new DynamicMethod(
				// 不需要为动态方法命名, 并且不能通过名称调用他们
				// 多个动态方法可以具有相同的名称, 名称将在调用堆栈中显示并且可用于调试
				"SquareIt",
				// 返回类型
				typeof(long),
				// 用于为动态方法指定参数类型的数组
				methodArgs,
				// 该方法与包含EmitDynamicMethodTests类的模块进行关联
				// 其实可以指定任何加载的模块, 动态方法的行为类似于模块级别的static方法
				typeof(EmitDynamicMethodTests).Module);

			// 发出方法主体, 此例使用ILGenerator对象发出IL
			// 也可以使用DynamicILInfo对象与非托管代码生成器发出
			ILGenerator il = squareIt.GetILGenerator();
			// 将参数0放置到求值堆栈上
			il.Emit(OpCodes.Ldarg_0);
			// 将求值堆栈栈顶的数字转换成int64
			il.Emit(OpCodes.Conv_I8);
			// 拷贝求值栈顶的值, 然后压回求值栈
			il.Emit(OpCodes.Dup);
			// 倍乘两个值, 将结果压入栈
			il.Emit(OpCodes.Mul);
			// 从当前方法返回, 如果求值堆栈中有值, 将值放入调用者的求值堆栈
			il.Emit(OpCodes.Ret);
			// 通过调用CreateDelegate方法创建表示动态方法的委托的实例
			// 创建委托即表示该方法的声明已经完成, 后续任何更改方法的尝试都将被忽略
			var invokeSquareIt =
				(OneParameter<long, int>)
				squareIt.CreateDelegate(typeof(OneParameter<long, int>));
			var p = 3;
			var actual = invokeSquareIt(p);
			Assert.Equal(9, actual);
		}

		/// <summary>
		/// 定义和执行绑定到对象的动态方法
		/// </summary>
		[Fact]
		public void EmitDynamicMethod_BindToClass()
		{
			// 创建动态方法
			// 如果方法的委托要绑定到对象, 第一个参数必须与委托要绑定到的类型匹配
			var methodArgs = new[] { GetType(), typeof(int) };
			var multiplyHidden = new DynamicMethod(
				// 名称
				string.Empty,
				// 返回类型
				typeof(int),
				// 参数
				methodArgs,
				// 绑定到的类型
				GetType());

			// 发出动态方法
			ILGenerator iLGenerator = multiplyHidden.GetILGenerator();
			// 将类型参数放到堆栈
			iLGenerator.Emit(OpCodes.Ldarg_0);
			var testInfo = GetType().GetField("test", BindingFlags.NonPublic | BindingFlags.Instance);
			// 寻找当前求值堆栈上引用的对象的一个字段值
			iLGenerator.Emit(OpCodes.Ldfld, testInfo);
			// 将第二个参数加载到堆栈上
			iLGenerator.Emit(OpCodes.Ldarg_1);
			// 将俩个值相乘
			iLGenerator.Emit(OpCodes.Mul);
			// 将结果返回
			iLGenerator.Emit(OpCodes.Ret);
			// 创建委托
			OneParameter<int, int> invoker =
				(OneParameter<int, int>)
				multiplyHidden.CreateDelegate(
					typeof(OneParameter<int, int>),
					Activator.CreateInstance(GetType()));
			var actual = invoker(3);
			Assert.Equal(3 * 42, actual);
		}
	}
}
