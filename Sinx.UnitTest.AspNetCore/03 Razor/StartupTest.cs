using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._03_Razor
{
	public class StartupTest
	{
		[Fact]
		public async Task Razor_RenderAsync()
		{
			var render = RazorRenderFactory.Create();
			var content = await render.RenderAsync(Path.Combine(Directory.GetCurrentDirectory(), "03 Razor", "Template.cshtml"), Enumerable.Range(0, 9));
			File.WriteAllText("View.html", content);
		}

	}
}
