using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.Base
{
	/// <summary>
	/// 引用类型和值类型
	/// </summary>
	/// <remarks>
	/// 值类型(结构) -> System.ValueType -> System.Object
	/// 值类型(枚举) -> System.Enum -> System.ValueType -> System.Object
	/// 值类型两种形式: 未装箱 / 已装箱
	/// 装箱过程: 在托管堆中分配内存 => 字段复制 => 返回对象地址
	/// 引用类型总是已装箱模式
	/// System.ValueType 与 System.Object 有相同的方法, 
	///		. 重写了 Equals(), 只要对象的字段值完全匹配, 返回true. 
	///		. 重写了 GetHashCode(), 算法有些复杂, 但是会将对象的字段值考虑进去.
	/// </remarks>
	public class ReferenceTypeAndValueTypeTests
	{
		[Fact]
		public void ValueType_InheritObject_Yes()
		{
			ValueType vt = 1;
			object o = vt;
			Assert.Equal(o, 1);
		}

		[Fact]
		public void ValueType_InheritValueType_Yes()
		{
			int a = 1;
			ValueType v = a;
			Assert.Equal(v, 1);
			StringComparison sc = StringComparison.OrdinalIgnoreCase;
			Enum e = sc;
			Assert.Equal(e, StringComparison.OrdinalIgnoreCase);
			v = sc;
			Assert.Equal(v, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void ValueType_Packing_IsReference()
		{
			object a = 1;
			Assert.True(a is ValueType);
			var b = 1.ToString();
			Assert.True(a is ValueType);
		}

		[Fact]
		public void ReferenceType_InheritValueType_CompileError()
		{
			//ValueType v = "123";
		}
	}
}
