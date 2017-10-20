using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;

namespace Sinx.UnitTest.AspNetCore._03_Razor
{
    public class RazorRenderFactory
    {
		public static IRender Create()
		{
			var sc = new ServiceCollection();
			sc.AddSingleton(PlatformServices.Default.Application);
			var appDir = Directory.GetCurrentDirectory();
			var hostEnvironment = new HostingEnvironment
			{
				 WebRootFileProvider = new PhysicalFileProvider(appDir)
			};
			sc.AddSingleton<IHostingEnvironment>(hostEnvironment);
			sc.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
			sc.AddSingleton<DiagnosticSource>(new DiagnosticListener("Microsoft.AspNetCore"));
			sc.AddLogging();
			sc.AddMvc();
			sc.AddSingleton<IRender, RazorRender>();
			var sp = sc.BuildServiceProvider();
			return sp.GetService<IRender>();
		}
    }
}
