using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.System
{
    public class SystemObjectTest
    {
	    class C : object
	    {
			public string Name { get; set; }
			public int Age { get; set; }

		    public C Clone()
		    {
				// 1. 受保护, 非虚方法
				// 2. 浅拷贝, 非托管代码实现
			    return this.MemberwiseClone() as C;
		    }
	    }
		[Fact]
	    public void System_Object_MemberwiseClone()
	    {
		    var c = new C
		    {
			    Name = "sinx",
			    Age = 23
		    };
		    var cCopy = c.Clone();
		    Assert.Equal(c.Name, cCopy.Name);
		    Assert.Equal(c.Age, cCopy.Age);
	    }
    }
}
