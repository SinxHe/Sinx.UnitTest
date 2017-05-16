using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sinx.UnitTest.Infrastructure.Repository;
using Sinx.UnitTest.UI.AspNetCore.Models;

namespace Sinx.UnitTest.UI.AspNetCore.Apis
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class ArticlesController : Controller
	{
		private readonly ArticleRepository _repo;
		public ArticlesController(ArticleRepository repo)
		{
			_repo = repo;
		}

		[HttpGet]
		public async Task<IEnumerable<DTO_Article>> GetAsync(int index = 1, int size = 20)
		{
			var articles = await _repo.GetAsync(index, size);
			var dtoArticles = articles.Select(a => new DTO_Article
			{
				Id = a.Id,
				Title = a.Title,
				Content = a.Content,
				CreateTime = a.CreateTime
			});
			return dtoArticles;
		}

		[HttpGet("{id}")]
		public async Task<DTO_Article> GetAsync([FromRoute]int id)
		{
			var article = await _repo.GetAsync(id);
			var dtoArticles = new DTO_Article
			{
				Id = article.Id,
				Title = article.Title,
				Content = article.Content,
				CreateTime = article.CreateTime
			};
			return dtoArticles;
		}
	}
}