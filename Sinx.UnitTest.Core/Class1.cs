using System;
using Xunit;

namespace Sinx.UnitTest.Core
{
    public class Class1
    {
		[Fact]
	    public void Run()
	    {
		    var a = 1;
			Assert.Equal(a, 1);
	    }
    }
}
