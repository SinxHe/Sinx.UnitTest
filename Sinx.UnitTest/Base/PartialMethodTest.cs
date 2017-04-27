using Xunit;

namespace Sinx.UnitTest.Base
{
	public class PartialMethodTest
	{
		[Fact]
		public void PartialMethod()
		{
			var a = new Base{Name = "Sinx"};
			Assert.Equal(a.Name, "SINX");
			var b = new Derived { Name = "Sinx" };
			Assert.Equal(b.Name, "SINXOnNameChanging");

			var c = new Base { Name2 = "sinX" };
			Assert.Equal(c.Name2, "sinS");
		}

		// use class inheritance
		private partial class Base
		{
			private string _name;
			private string _name2;
			protected virtual string OnNameChanging(string name)
			{
				return name;
			}

			// 分部方法返回值必须是void, 而且必须是private, 但是C#编译器不让添加private标记
			partial void OnNameChanging2(ref string name);
			public string Name
			{
				get => _name;
				set
				{
					value = OnNameChanging(value.ToUpper());    // 如果有重写, 调用重写, 没有重写, 调用虚方法, 反正value.ToUpper()必须调用
					_name = value;
				}
			}

			public string Name2
			{
				get => _name2;
				set
				{
					OnNameChanging2(ref value); // 如果没有分布方法定义, CLR会擦除这里, 包括"()"里面的表达式
					_name2 = value;
				}
			}
		}

		private class Derived : Base
		{
			protected override string OnNameChanging(string name)
			{
				return name + nameof(OnNameChanging);
			}
		}
		// 问题1: 类型必须是非密封类, 也不能用于值类型(struct - 值类型隐式密封)
		// 问题2: 不能用于静态方法, 静态方法不能重写
		// 问题3: 效率问题, 定义一个类型只是为了重写一个方法
		// 问题4: 不重写OnNameChanging, 基类代码仍然调用一个什么都不做的虚方法
		// 问题5: 无论OnNameChanging是否访问传给他的实参, 编译器都会生成对ToUpper的调用IL

		private partial class Base
		{
			partial void OnNameChanging2(ref string name)
			{
				name = name.Replace('X', 'S');
			}
		}
	}
}
