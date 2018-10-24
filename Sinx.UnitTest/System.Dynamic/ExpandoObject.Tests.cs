using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Xunit;

// ReSharper disable PossibleNullReferenceException
namespace Sinx.UnitTest.System.Dynamic
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// ExpandoObject 是.Net4.0在BCL中的类, 他的作用是利用.Net中定义的动态分发功能, 定义一个可任意拓展的类型
	/// </remarks>
	public class ExpandoObjectTests
	{
		private readonly dynamic _d;
		public ExpandoObjectTests()
		{
			_d = new ExpandoObject();
			_d.Id = 1;
			_d.Name = "sinx";
		}

		[Fact]
		public void ExpandoObject_Reflection()
		{
			var prop = _d.GetType().GetProperty("Name");
			Assert.NotEqual(prop, "sinx");
		}

		[Fact]
		public void ExpandoOjbect_ConvertToIEnumerable_Success()
		{
			var id = ((IEnumerable<KeyValuePair<string, object>>)_d).First().Key;
			var idValue = ((IEnumerable<KeyValuePair<string, object>>) _d).First().Value;
			Assert.Equal("Id", id);
			Assert.Equal(1, idValue);
		}

		[Fact]
		public void ExpandoObject_ConvertToIDictionary_Success()
		{
			var id = ((IDictionary<string, object>)_d).First().Key;
			var idValue = ((IDictionary<string, object>) _d).First().Value;
			Assert.Equal("Id", id);
			Assert.Equal(1, idValue);
		}
	}
}
