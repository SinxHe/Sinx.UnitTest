using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace Sinx.UnitTest.System.Dynamic
{
	// [UnitOfWorkName]_[ScenarioUnderTest]_[ExpectedBehavior]
	// [被测试的方法(s)]_[假设条件]_[期望的表现]
	public class DynamicTests
	{
		/// <summary>
		/// dynamic In: 普通类型 Result: 使用反射
		/// </summary>
		/// <remarks>
		/// 对于没有继承 IDynamicMetaObjectProvider 接口的普通类型, 运行时绑定的类会使用反射在对象上执行操作
		/// </remarks>
		[Fact]
		public void Dynamic_WrapObjectWithNoGetMetaData_UseReflection()
		{
			object set = new[] { 1, 2, 3 };
			dynamic d = set;
			Assert.Equal(d.Length, 3);
		}

		/// <summary>
		/// dynamic In: Delegate Condition: Type Cast Result: Successful  
		/// </summary>
		/// <remarks>
		/// 普通类型(固定类型)不可以给属性赋委托, 动态分发类型可以給属性赋委托
		/// </remarks>
		[Fact]
		public void Dynamic_AssignedDelegate_NeedTypeInference()
		{
			object o = (Func<int, int>)(a => 2 * a);
			//dynamic d = a => 2;
			dynamic d = (Func<int, int>)(a => 2 * a);
			Assert.Equal(d(0), 0);
			Assert.Equal(d(1), 2);
			Assert.Throws<RuntimeBinderException>(() =>
			{
				d.del = (Func<int, int>) (a => 2 * a);
			});
			dynamic d2 = new ExpandoObject();
			d2.Name = "测试各种属性添加";
			d2.del = (Func<int, int>)(a => 2 * a);
			Assert.Equal(d2.Name, "测试各种属性添加");
			Assert.Equal(d2.del(2), 4);
		}
	}
}
