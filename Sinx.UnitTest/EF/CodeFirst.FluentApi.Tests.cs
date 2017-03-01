using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.EF
{
	public class CodeFirstWithFluentApiTests
	{
		private readonly BlogContext _dbContext = new BlogContext();

		private const string ConnectionString =
			"Data Source=10.1.1.2;Initial Catalog=TestDb;User Id=zxxktest ;Password=123456";
		[Fact]
		public void CreateDb_ValidRelationByFlentApi_Success()
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
					", (SqlConnection) conn))
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
				Url = nameof(Blog.Url),
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
			{ }
			public DbSet<Blog> Blogs { get; set; }
			public DbSet<Post> Posts { get; set; }
			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				// EF6: https://msdn.microsoft.com/en-us/library/jj591620(v=vs.113).aspx
				// 总结:
				//		1. 引用导航:
				//			1. HasRequired - 1(required)对(见with)
				//			2. HasOptional - (0/1)(optional)对(见with)
				//			3. HasMany	   - n(required)对(见with)
				//		2. 反向导航:
				//			1. HasRequired -> 
				//				1. WithMany WithOptional WithRequiredPrincipal WithRequiredDependent
				//			2. HasOptional -> 
				//				1. WithMany WithOptionalPrincipal WithOptionalDependent WithRequired
				//			3. HasMany ->
				//				1. WithMany WithOptional WithRequired
				//		3. 大多情况, EF可以推断那个是主体, 那个是依赖体, 然而, 当两边都是Required或都是Optional的时候他需要你指定
				modelBuilder.Entity<Blog>()
					.HasKey(e => e.BlogId)		// 设置主键
					.HasMany(e => e.Posts)
					.WithRequired(e => e.Blog)
					.HasForeignKey(e => e.BlogId)
					;
				modelBuilder.Entity<Post>()
					.HasKey(e => e.PostId)		// 设置主键
					; 
			}
		}

		public class Blog
		{
			public int BlogId { get; set; }
			public string Url { get; set; }

			public List<Post> Posts { get; set; }
		}

		public class Post
		{
			public int PostId { get; set; }
			public string Title { get; set; }
			public string Content { get; set; }
			public int BlogId { get; set; }
			public Blog Blog { get; set; }
		}
	}

	
}
