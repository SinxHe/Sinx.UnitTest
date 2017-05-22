using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sinx.UnitTest.Domain.Model;
using Sinx.UnitTest.Infrastructure.Repository;

namespace Sinx.UnitTest.UI.AspNetCore
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<AppDbContext>(
				optionsBuilder => optionsBuilder.UseInMemoryDatabase());

			services.AddMvc();

			services.AddScoped<IBrainstormSessionRepository,
				EFStormSessionRepository>();
			services.AddSingleton<IDbConnection>(new SqlConnection("Data Source=neter.me;Initial Catalog=DotNetStudioDb;User Id=sa;Password=SinxHe*#7370#"));
			services.AddSingleton(typeof(ArticleRepository), sp => new ArticleRepository(sp.GetService<IDbConnection>()));
		}

		public void Configure(IApplicationBuilder app,
			IHostingEnvironment env,
			ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(LogLevel.Warning);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				var repository = app.ApplicationServices.GetService<IBrainstormSessionRepository>();
				InitializeDatabaseAsync(repository).Wait();
			}

			app.UseStaticFiles();

			app.UseMvc();
		}

		public async Task InitializeDatabaseAsync(IBrainstormSessionRepository repo)
		{
			var sessionList = await repo.ListAsync();
			if (!sessionList.Any())
			{
				await repo.AddAsync(GetTestSession());
			}
		}

		public static BrainstormSession GetTestSession()
		{
			var session = new BrainstormSession()
			{
				Name = "Test Session 1",
				DateCreated = new DateTime(2016, 8, 1)
			};
			var idea = new Idea()
			{
				DateCreated = new DateTime(2016, 8, 1),
				Description = "Totally awesome idea",
				Name = "Awesome idea"
			};
			session.AddIdea(idea);
			return session;
		}
	}
}