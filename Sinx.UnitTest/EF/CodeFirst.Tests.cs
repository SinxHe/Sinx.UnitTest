using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.EF
{
	public class CodeFirstTests
	{
		private readonly BlogContext _dbContext = new BlogContext();

		[Fact]
		public void CreateDb_ValidRelation_Success()
		{
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
				: base("Data Source=10.1.1.2;Initial Catalog=TestDb;User Id=zxxktest ;Password=123456")
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
