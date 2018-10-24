using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace Sinx.UnitTest.System.Dynamic
{
	// [UnitOfWorkName]_[ScenarioUnderTest]_[ExpectedBehavior]
	// [被测试的方法(s)]_[假设条件]_[期望的表现]
	public class DynamicTests
	{
		/// <summary>
		/// 对于局部变量dynamic
		/// </summary>
		/// <remarks>
		/// 只生成object类型IL, 但是不会使用DynamicAttribute进行标记
		/// </remarks>
		[Fact]
		public void Dynamic_LocalVar_WithoutDynamicAttribute()
		{
			// IL => object d = 123;
			// 局部dynamic变量payload(特殊的IL)没有DynamicAttribute, 因为它限制在方法内部使用
			// DynamicAttribute 指明一个Object是动态分发类型
			dynamic d = 123;
		}

		private dynamic _d;
		[Theory]
		[InlineData(456)]
		public void Dynamic_Property_UseDynamicAttribute(dynamic d)
		{
			// _d 和 d 由于都添加了DynamicAttribute标识
			_d = d;
		}

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
			// 可以看到IL中: ldstr        "Length", 说明使用反射在调用
			var length = d.Length;
			Assert.Equal(length, 3);
		}

		[Fact]
		public void Dynamic_WrapObjectWithGetMetaDataType_UseGetMetaObjectMethod()
		{
			dynamic d = new ExpandoObject();    // 实现了IDynamicMetaObjectProvider, 所以应该调用GetMetaObject方法而不是使用反射
			d.Name = "sinx";
			var name1 = d.Name; // object
			string name2 = d.Name;  // string
									// TODO: 这里好像使用的也是反射啊
		}

		private static readonly int StaticMember = 1;
		/// <summary>
		/// dynamic 访问静态成员 - 异常
		/// 其实正常代码中也只能使用 SomeType.StaticMember
		/// </summary>
		[Fact]
		public void Dynamic_Static_GetStaticMember_Fail()
		{
			dynamic d = this;
			//Assert.Equal(this.StaticMember, 1);
			Assert.Equal(1, StaticMember);  // 不能通过实例调用, 但是在方法中直接访问可以
			Assert.Throws<RuntimeBinderException>(() => d.StaticMember);
		}

		/// <summary>
		/// dynamic访问静态成员
		/// </summary>
		/// <remarks>
		/// dynamic 限制只能访问对象的实例成员, 所以这里添加了拓展
		/// </remarks>
		[Fact]
		public void Dynamic_InvokeStaticMember()
		{
			dynamic stringType = new StaticMemberDynamicWrapper(typeof(string));
			var r = stringType.Concat("A", "B");	// 动态调用String的静态Concat方法
			Assert.Equal("AB", r);

			dynamic d = new StaticMemberDynamicWrapper(this.GetType());
			Assert.Equal(d.StaticMember, 1);
		}

		private sealed class StaticMemberDynamicWrapper : DynamicObject
		{
			private readonly TypeInfo _typeInfo;

			public StaticMemberDynamicWrapper(Type type)
			{
				_typeInfo = type.GetTypeInfo();
			}

			public override IEnumerable<string> GetDynamicMemberNames()
			{
				return _typeInfo.DeclaredMembers.Select(m => m.Name);
			}

			public override bool TryGetMember(GetMemberBinder binder, out object result)
			{
				result = null;
				var field = FindField(binder.Name);
				if (field != null)
				{
					result = field.GetValue(null);
					return true;
				}

				var prop = FindProperty(binder.Name, true);
				if (prop != null)
				{
					result = prop.GetValue(null, null);
					return true;
				}
				return false;
			}

			public override bool TrySetMember(SetMemberBinder binder, object value)
			{
				var field = FindField(binder.Name);
				if (field != null)
				{
					field.SetValue(null, value);
					return true;
				}
				var prop = FindProperty(binder.Name, false);
				if (prop != null)
				{
					prop.SetValue(null, value, null);
					return true;
				}
				return false;
			}

			public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
			{
				MethodInfo method = FindMethod(binder.Name, args.Select(c => c.GetType()).ToArray());
				if (method == null)
				{
					result = null;
					return false;
				}
				result = method.Invoke(null, args);
				return true;
			}

			private MethodInfo FindMethod(string name, Type[] paramTypes)
			{
				return _typeInfo.DeclaredMethods.FirstOrDefault(mi => mi.IsPublic  
					&& mi.IsStatic 
					&& mi.Name == name 
					&& ParametersMatch(mi.GetParameters(), paramTypes));
			}

			private bool ParametersMatch(ParameterInfo[] parameters, Type[] paramTypes)
			{
				if (parameters.Length != paramTypes.Length)
				{
					return false;
				}
				return !parameters.Where((t, i) => t.ParameterType != paramTypes[i]).Any();
			}

			private PropertyInfo FindProperty(string name, bool get)
			{
				if (get)
				{
					return _typeInfo.DeclaredProperties.FirstOrDefault(
						pi => pi.Name == name && pi.GetMethod != null &&
						pi.GetMethod.IsPublic && pi.GetMethod.IsStatic);
				}
				return _typeInfo.DeclaredProperties.FirstOrDefault(
					pi => pi.Name == name && pi.SetMethod != null && 
					pi.SetMethod.IsPublic && pi.SetMethod.IsPublic);
			}

			private FieldInfo FindField(string name)
			{
				return _typeInfo.DeclaredFields.FirstOrDefault(fi => fi.IsPublic && fi.IsStatic
					&& fi.Name == name);
			}
		}
	}
}
