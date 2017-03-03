using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Linq
{
	public class EnumerableTests
	{
		#region Select
		[Fact]
		public void SelectTest_Array_ArrayValueAndIndex()
		{
			var array = new[] {"sinx", "he"};

			var ie0 = array.Select((m, i) => new {m,i}).ToArray();
			Assert.Equal(ie0[0].m, "sinx");
			Assert.Equal(ie0[0].i, 0);
			Assert.Equal(ie0[1].m, "he");
			Assert.Equal(ie0[1].i, 1);

			var ie1 = array.Where(m => m == "he").Select((m, i) => new {m,i}).First();	// m 是元素, i 是元素的索引 
			Assert.Equal(ie1.m, "he");
			Assert.Equal(ie1.i, 0); // NOTICE: 是where之后生成的元素序列, 所以这里是0
		}

		#endregion
	}
}
