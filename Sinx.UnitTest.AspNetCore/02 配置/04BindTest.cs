using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Sinx.UnitTest.AspNetCore._02_配置
{
	public class _04BindTest
	{
		[Fact]
		public void Bind_()
		{
			var dict = new Dictionary<string, string>
			{
				{"Profile:MachineName", "Rick"},
				{"App:MainWindow:Height", "11"},
				{"App:MainWindow:Width", "11"},
				{"App:MainWindow:Top", "11"},
				{"App:MainWindow:Left", "11"}
			};
			var conf = new ConfigurationBuilder()
				.AddInMemoryCollection(dict)
				.Build();
			var window = new MyWindow { Name = "123" };
			conf.GetSection("App:MainWindow").Bind(window);
			Assert.Equal("123", window.Name);
			Assert.Equal(11, window.Height);
			Assert.Equal(11, window.Width);
		}

		[Fact]
		public void Bind_Array()
		{
			var dic = new Dictionary<string, string>
			{
				{ "Array:0", "Element0"},
				{ "Array:1", "Element1"},
				{ "Height", "33"}
			};
			var conf = new ConfigurationBuilder()
				.AddInMemoryCollection(dic)
				.Build();
			var array = conf.GetValue<string[]>("Array");
			Assert.Null(array);
			var window = new MyWindow();
			conf.Bind(window);
			Assert.NotEmpty(window.Array);
			Assert.Equal(window.Height, 33);
		}

		public class MyWindow
		{
			public string Name { get; set; }
			public int Height { get; set; } = 22;
			public int Width { get; set; }
			public int Top { get; set; }
			public int Left { get; set; }
			public string[] Array { get; set; }
		}
	}
}
