using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
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
			dynamic test = ipy.UseFile(@"IronPythonTests\hello_world.py");

			// Act
			var str = test.GetString();

			// Assert
			Assert.Equal("hello world", str);
		}

		[Fact]
		public void IronPython_StdLibUsage()
		{
			// Arrange
			var setup = Python.CreateRuntimeSetup(null);
			var runtime = new ScriptRuntime(setup);
			var engine = Python.GetEngine(runtime);
			var paths = engine.GetSearchPaths();
			paths.Add(@"Lib\");
			engine.SetSearchPaths(paths);

			// Act
			var source = engine.CreateScriptSourceFromFile(@"IronPythonTests\use_stdlib.py");
			var scope = engine.CreateScope();
			source.Execute(scope);
			var result = scope.GetVariable("result");

			// Assert
			Assert.Equal("{\"a\": 0, \"b\": 0, \"c\": 0}", result);
		}

		[Fact]
		public void IronPython_3rdPartyLibUsage()
		{
			// Arrange
			var setup = Python.CreateRuntimeSetup(null);
			var runtime = new ScriptRuntime(setup);
			var engine = Python.GetEngine(runtime);
			var paths = engine.GetSearchPaths();
			paths.Add(@"Lib\");
			paths.Add(@"IronPythonTests\Modules\");
			engine.SetSearchPaths(paths);

			// Act
			var source = engine.CreateScriptSourceFromFile(@"IronPythonTests\use_3rd_party_lib.py");
			var scope = engine.CreateScope();
			source.Execute(scope);
			var result = scope.GetVariable("result");

			Assert.Equal(result, 200);
		}
	}
}
