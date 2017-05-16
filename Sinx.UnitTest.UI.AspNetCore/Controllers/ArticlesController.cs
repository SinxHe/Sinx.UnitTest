using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sinx.UnitTest.Infrastructure.Repository;
using Sinx.UnitTest.UI.AspNetCore.Models;

namespace Sinx.UnitTest.UI.AspNetCore.Controllers
{
	[Route("[controller]")]
    public class ArticlesController : Controller
    {
	    private readonly ArticleRepository _repo;
	    public ArticlesController(ArticleRepository repo)
	    {
		    _repo = repo;
	    }
		public async Task<IActionResult> List(int index = 1, int size = 20)
        {
	        var article = await _repo.GetAsync(index, size);
	        var dtoArticle = article.Select(a => new DTO_Article
	        {
		        Id = a.Id,
		        Title = a.Title,
		        Content = a.Content,
		        CreateTime = a.CreateTime
	        });
			return View(dtoArticle);
        }

		[HttpGet("{id:int}")]
	    public async Task<IActionResult> Single([FromRoute]int id)
	    {
		    var article = await _repo.GetAsync(id);
		    var dtoArticle = new DTO_Article
		    {
			    Id = article.Id,
			    Title = article.Title,
			    Content = article.Content,
			    CreateTime = article.CreateTime
		    };
		    return View(dtoArticle);
		}
    }
}