using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Sinx.UnitTest.Domain.Model;
using Dapper;

namespace Sinx.UnitTest.Infrastructure.Repository
{
    public class ArticleRepository
    {
	    private readonly IDbConnection _db;
		public ArticleRepository(IDbConnection db)
		{
			_db = db;
		}

	    public async Task<Article> GetAsync(int id)
	    {
		    var task = _db.QueryAsync<Article>($"SELECT * FROM Article WHERE Id = {id}");
			var articles = await task;
		    return articles.FirstOrDefault();
	    }

	    public Task<IEnumerable<Article>> GetAsync(int index, int size)
	    {
		    var task = _db.QueryAsync<Article>($"SELECT * FROM(SELECT *, ROW_NUMBER() OVER(ORDER BY Id) AS RowNum FROM dbo.Article) AS T WHERE T.RowNum BETWEEN {(index - 1) * size} AND {size}");
			return task;
	    }

	    public async Task<bool> AddAsync(Article article)
	    {
		    const string sql = @"INSERT INTO [dbo].[Article]
							([Title]
							,[Content])
						VALUES
							(@Title
							,@Content)";
		    var task = _db.ExecuteAsync(sql, article);
			return await task == 1;
	    }
    }
}
