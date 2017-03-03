using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Linq.Expressions
{
	/// <summary>
	/// TODO : https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
	/// </summary>
	public class BinaryExpressionTest
	{
		[Theory]
		[InlineData(true, true, true)] 
		[InlineData(false, true, false)]
		[InlineData(true, false, false)]
		[InlineData(false, false, false)]
		public void Binary_BoolCondition_ExpectOk(bool left, bool right, bool expect)
		{
			Expression leftB = Expression.Constant(left); 
			Expression rightB = Expression.Constant(right);
			var and = Expression.And(leftB, rightB);
			var resultLambda = Expression.Lambda<Func<bool>>(and);
			var result = resultLambda.Compile()();
			Assert.Equal(result, expect);
		}
	}
}
