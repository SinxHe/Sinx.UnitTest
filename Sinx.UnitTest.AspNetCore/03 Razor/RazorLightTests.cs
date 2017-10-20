using System.IO;
using System.Linq;
using RazorLight;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._03_Razor
{
    public class RazorLightTests
    {
		[Fact]
		public void RazorLight_Render()
		{
			var engine = new EngineFactory().ForFileSystem(Path.Combine(Directory.GetCurrentDirectory()));
			var content = engine.CompileRenderAsync("Template.cshtml", Enumerable.Range(0, 9)).Result;
			File.WriteAllText("View.html", content);
		}
	}
}
