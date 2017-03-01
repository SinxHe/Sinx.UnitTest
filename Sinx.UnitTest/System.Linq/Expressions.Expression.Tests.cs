using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Linq
{
	/// <summary>
	/// 表达式测试
	/// </summary>
    public class ExpressionTests
    {
		[Theory]
		[InlineData]
	    public void BinaryAnd_SetBinaryBollean_ExpectOk(bool left, bool right, bool expect)
		{
			var l = Expression.And();
		}

    }
}
