using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable once InconsistentNaming
namespace Sinx.UnitTest.Base
{
	public class KeyWord_is_as
	{
		[Fact]
		public void KeyWord_Is_ConvertType()
		{
			// is never throw exception
			A a = new B();
			if (a is B)	// 检查能否转换
			{
				B b = (B)a;	// 再检查一次能否转换
			}
		}

		[Fact]
		public void KeyWord_As_ConvertType_BetterThanIs()
		{
			A a = new B();
			B b = a as B;	// 1. never throw exception 2. CLR only one type check
			if (b != null)
			{
				// do something
			}
		}

		class A{}

		class B : A{}
	}
}
