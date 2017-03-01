using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.EF
{
	public class CodeFirstTests
	{
		private readonly BlogContext _dbContext = new BlogContext();

		private const string ConnectionString =
			"Data Source=10.1.1.2;Initial Catalog=TestDb;User Id=zxxktest ;Password=123456";
		[Fact]
		public void CreateDb_ValidRelation_Success()
		{
			try
			{
				using (IDbConnection conn = new SqlConnection(ConnectionString))
				{
					conn.Open();
					using (var cmd = new SqlCommand(@"
						DROP TABLE [dbo].[__MigrationHistory];
						DROP TABLE [dbo].[Posts];
						DROP TABLE [dbo].[Blogs];
					", (SqlConnection)conn))
					{
						cmd.ExecuteNonQuery();
					}
					conn.Close();
				}
			}
			catch (Exception)
			{
				// ignore because table not exist
			}
			var blog = new Blog
			{
				//Url = nameof(Blog.Url),
				Rating = 1,
				Posts = new List<Post>{
					new Post
					{
						BlogId = 1,
						Content = nameof(Post.Content),
						Title = nameof(Post.Title)
					}
				}
			}; 
			_dbContext.Blogs.Add(blog);  
			_dbContext.SaveChanges();
		}
		public class BlogContext : DbContext
		{
			public BlogContext()
				: base(ConnectionString)
			{
			}
			// EF O/R 映射过程中发现 O 的过程: 通过调用DbContext中的DbSet Blogs发现Blog, 然后顺着Blog的导航发现Post
			public DbSet<Blog> Blogs { get; set; }
		}

		public class Blog
		{
			[Key]
			public int BlogId { get; set; }
			public string Url { get; } = "fdjklsafjdskla";  // NOTICE: 只包含Get没有Set的属性默认是不保存到数据库中的
			public int Rating { get; set; }

			[InverseProperty(nameof(Post.Blog))]    // 导航配对
			public List<Post> Posts { get; set; }   // 用Blog的视角 - 引用导航
		}

		public class Post
		{
			[Key]
			public int PostId { get; set; }
			public string Title { get; set; }
			public string Content { get; set; }

			public int BlogId { get; set; }

			[ForeignKey(nameof(BlogId))]    // 指定对应的外键 key
			public Blog Blog { get; set; }  // 用Blog的视角 - 反向导航
		}
	}
}
