using System;

namespace Sinx.UnitTest.Domain.Model
{
    public class Article
    {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime CreateTime { get; set; }
		public DateTime ModifyTime { get; set; }
	}
}
