using System;

namespace Sinx.UnitTest.UI.AspNetCore.ViewModels
{
	public class StormSessionViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTimeOffset DateCreated { get; set; }
		public int IdeaCount { get; set; }
	}
}