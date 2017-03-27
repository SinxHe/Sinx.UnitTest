using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Sinx.UnitTest.HBase
{
	public class Crud
	{
		private readonly HttpClient _client;
		public Crud()
		{
			_client = new HttpClient
			{
				BaseAddress = new Uri("http://10.1.1.26:8086"),
			};
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
		[Fact]
		public void Rest_Get()
		{
			var task = _client.GetAsync("t1/r1/f1:c1");
			var res = task.Result;
			var content = res.Content.ReadAsStringAsync().Result;
			var model = JsonConvert.DeserializeObject<Table>(content);
		}

		public class Table
		{
			public Row[] Row { get; set; }
		}

		public class Row
		{
			public string Key { get; set; }
			public Cell[] Cell { get; set; }
		}

		public class Cell
		{
			public string Column { get; set; }
			public long Timestamp { get; set; }
			[JsonProperty("$")]
			public string Value { get; set; }
		}
	}
}
