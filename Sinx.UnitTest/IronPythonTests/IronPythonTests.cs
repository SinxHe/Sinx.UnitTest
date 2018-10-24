using IronPython.Hosting;
using Xunit;

namespace Sinx.UnitTest.IronPythonTests
{
	public class IronPythonTests
	{
		[Fact]
		public void IronPython_OnlyCode()
		{
			// Arrange
			var ipy = Python.CreateRuntime();
			dynamic test = ipy.UseFile(@"IronPythonTests\hello-world.py");

			// Act
			var str = test.GetString();

			// Assert
			Assert.Equal("hello world", str);
		}
	}
}
