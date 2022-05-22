using System;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Sinx.InvokeCpp.CSharp
{
	public class DeliveryString
	{
		[Fact]
		public unsafe void ToCppTest()
		{
			var str0 = "123";
			var str1 = "456";
			var str = Marshal.PtrToStringAnsi(Interface.ToCpp(str0, str1));
			var ref0 = __makeref(str0);
			var ref1 = __makeref(str1);
			var ptr0 = **(IntPtr**)(&ref0);				
			var ptr1 = **(IntPtr**)(&ref1);
		}

		[Fact]
		public void DeliveryIntTest()
		{
			var i = 123;
			var newI = Interface.DeliveryInt(123);
			Assert.Equal(i, newI);
		}

		[Fact]
		public void DeliveryStringTest()
		{
			var msg = "hello";
			var ptr = Interface.DeliveryString(msg);
			Assert.NotNull(ptr);
			var str = Marshal.PtrToStringAnsi(ptr);
			Assert.Equal("abc", str);
			Interface.Free(ptr);
			str = Marshal.PtrToStringAnsi(ptr);
			Assert.NotEqual("abc", str);
		}

		[Fact]
		public void GetStringNewTest()
		{
			var ptr = Interface.GetStringNew();
			var str = Marshal.PtrToStringAnsi(ptr);
			Assert.Equal("abc", str);
			Interface.Delete(ptr);
			str = Marshal.PtrToStringAnsi(ptr);
			Assert.NotEqual("abc", str);
		}

		[Fact]
		public void ChangeStringTest()
		{
			// string类型是不可变类型, 创建之后不能改变, 所以平台调用会复制字符串
			var msg = "nihao, shijie";
			var size = 200;
			// 由于需要改变字符串的值, 则应该是用StringBuilder, StringBuilder要确保缓存区大小足够
			var sb = new StringBuilder(size);
			sb.Append(msg);
			Interface.ChangeString(msg, sb, size);
			Assert.Equal(msg, sb.ToString());
		}
	}

	public class Interface
	{
		private const string Path = @"D:\code\Sinx.UnitTest\x64\Debug\Sinx.InvokeCpp.Cpp.dll";

		[DllImport(Path)]
		public static extern IntPtr ToCpp(string str0, string str1);

		// 如果返回类型写string, 则直接Abort
		// 托管代码将一个字符串(Unicode)作为ANSI格式的字符串传递给非托管代码, CLR会先对
		// 该字符串进行复制, 然后将复制的字符串转化为ANSI字符串, 最后将字符串的内存地址传递
		// 给非托管代码.
		[DllImport(Path)]
		public static extern IntPtr DeliveryString(string msg);
		
		[DllImport(Path)]
		public static extern void ChangeString(string raw, StringBuilder sb, int size);

		[DllImport(Path)]
		public static extern IntPtr GetStringNew();

		[DllImport(Path)]
		public static extern void Delete(IntPtr ptr);

		[DllImport(Path)]
		public static extern void Free(IntPtr ptr);
		
		[DllImport(Path)]
		public static extern int DeliveryInt(int i);
		
	}
}
