using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System.Linq.Expressions
{
	public class ExpressTests
	{
		[Theory]
		[InlineData(1,2,3)]
		public void Expression_IntAddInt_AddResult(int left, int right, int expect)
		{
			// 创建逻辑块, 将其表示成普通的对象
			Expression leftExp = Expression.Constant(left);				// 创建常数
			Expression rightExp = Expression.Constant(right);			// 创建常数
			Expression addExp = Expression.Add(leftExp, rightExp);		// 创建相加逻辑
			Expression funcExp = Expression.Lambda<Func<int>>(addExp);  // 创建执行逻辑
			Func<int> resultLogicCode = ((Expression<Func<int>>) funcExp).Compile();	// 编译成可执行的"真实"代码
			Assert.Equal(addExp.ToString(), $"({left} + {right})");		// 生成了逻辑代码 (1 + 2)
			Assert.Equal(resultLogicCode(), expect);
		}
		[Theory]
		[InlineData(1, 2, 3)]
		public void Expression_IntAddIntWithLambda_AddResult(int left, int right, int expect)
		{
			Expression<Func<int>> funcExp = () => left + right;
			Func<int> resultFunc = funcExp.Compile();
			Console.WriteLine(funcExp.ToString());
			Assert.True(Regex.IsMatch(funcExp.ToString(), @"\(\)\s*=>\s*.+?left.+?\+.+?right"));
			Assert.Equal(resultFunc(), expect);
		}

		[Theory]
		[InlineData("left", "le", true)]
		public void Expression_LambdaExpWithMethod_RuntimePass(string left, string right, bool expect)
		{
			Expression<Func<string,  string, bool>> predExp = (l, r) => l.StartsWith(r);
			Func<string, string, bool> predDele = predExp.Compile();
			Assert.Equal(predDele(left, right), expect); 
		}

		[Theory]
		[InlineData("left", "le", true)]
		public void Expression_InvokeMethodFromExpWrapped_RuntimePass(string left, string right, bool expect)
		{
			ParameterExpression target = Expression.Parameter(typeof(string), left);
			ParameterExpression methodArg = Expression.Parameter(typeof(string), right);
			MethodInfo methodInfo = typeof(string).GetMethod(nameof(string.StartsWith), new[] {typeof(string)});
			Expression[] methodArgs = { methodArg };
			Expression call = Expression.Call(target, methodInfo, methodArgs);

			ParameterExpression[] lambdaParameters = {target, methodArg};
			var lambda = Expression.Lambda<Func<string, string, bool>>(call, lambdaParameters);
			var compiled = lambda.Compile();
			Assert.Equal(compiled(left, right), expect);
			// <<深入理解C#>> P.216
		}
	}
}
