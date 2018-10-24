using Xunit;

namespace Sinx.UnitTest.Base
{
	/// <summary>
	/// 对于类型实例构造时执行顺序的总结
	/// 1. 字段的内联初始化其实也是发生在构造函数中的, 但是在有多个构造函数中的时候小心代码膨胀
	/// 2. 内联初始化完成才会进行基类的构造函数的调用
	/// 3. 基类构造函数调用完成才会调用程序员写的构造器初始化调用
	/// 4. 尽量避免构造器函数中调用虚(抽象)方法, 因为这个时候派生类还没有初始化完成呢
	/// 5. 代码层面
	///		ctor(){
	///			内联初始化
	///			基类.ctor()
	///			构造器初始化
	///		}
	/// </summary>
	public class CtorTest
	{
		private static string _order = "";
		#region 基类派生类构造函数执行顺序
		class A
		{
			protected A()
			{
				_order += "a";
			}
		}

		class B : A
		{
			public B() : base()
			{
				_order += "b";
			}
		}
		/// <summary>
		/// -> B.Ctor开始 -> A.Cotr开始 -> A.Ctor结束 -> B.Ctor方法体 -> B.Ctor结束
		/// </summary>
		[Fact]
		public void Ctor_CtorCallOrder()
		{
			_order = "";
			var b = new B();
			Assert.Equal("ab", _order);
		}
		#endregion

		#region 内联初始化与构造函数初始化顺序
		class C
		{
			private int c = 1;
			public C()
			{
				if (c == 1)
				{
					_order += "inline";
				}
				c = 2;
			}
		}

		/// <summary>
		/// 内联初始化
		/// </summary>
		[Fact]
		public void Ctor_CotrCallOrder_InlineCotr()
		{
			_order = "";
			var c = new C();
			Assert.Equal("inline", _order);
		}
		#endregion

		#region 字段初始化与基类派生类构造函数执行顺序

		abstract class Father
		{
			protected int f = 1;

			protected abstract int GetS();
			protected Father()
			{
				if (GetS() == 1)
				{
					_order += " s_father ";
				}
				if (f == 1)
				{
					_order += " f ";
				}
				_order += " father ";
				f = 2;
			}
		}

		class Son : Father
		{
			private int s = 1;
			public Son() : base()
			{
				if (f == 2)
				{
					_order += " f_son ";
				}
				if (s == 1)
				{
					_order += " s ";
				}
				_order += " son ";
			}

			protected override int GetS()
			{
				return s;
			}
		}

		[Fact]
		public void Ctor_CtorOrderInClassDriveAndFieldInitOrder()
		{
			_order = "";
			var s = new Son();
			Assert.Equal(" s_father  f  father  f_son  s  son ", _order);
		}

		#endregion
	}
}
