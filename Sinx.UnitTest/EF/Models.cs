using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinx.UnitTest.EF.Models
{
	/// <summary>
	/// 文章主体 - 博客
	/// </summary>
	public class Blog
	{
		public int BlogId { get; set; }
		public string Url { get; set; }

		public List<Post> Posts { get; set; }   // 导航属性 - 引用导航[ReferenceNavigation]
	}

	/// <summary>
	/// 博客依赖体 - 文章
	/// </summary>
	public class Post
	{
		public int PostId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public int BlogForeignKey { get; set; }

		[ForeignKey(nameof(BlogForeignKey))]
		public Blog Blog { get; set; }  // 导航属性 - 反向导航[InverseProperty]

		public int AuthorUserId { get; set; }
		[ForeignKey(nameof(AuthorUserId))]
		public User Author { get; set; }

		public int ContributorUserId { get; set; }
		[ForeignKey(nameof(ContributorUserId))]
		public User Contributor { get; set; }
	}

	public class User
	{
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		[InverseProperty(nameof(Post.Author))]
		public List<Post> AuthroedPosts { get; set; }
		[InverseProperty(nameof(Post.Contributor))]
		public List<Post> ContributedToPosts { get; set; }
	}
}
