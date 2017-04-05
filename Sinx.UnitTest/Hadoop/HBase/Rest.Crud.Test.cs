using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Xunit;

namespace Sinx.UnitTest.Hadoop.HBase
{
	public class Crud
	{
		private readonly HttpClient _client;
		private readonly Func<string> _getNowDateTime = () => DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
		private readonly Func<string, string> _toBase64 = str => Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		public Crud()
		{
			_client = new HttpClient
			{
				BaseAddress = new Uri("http://10.1.1.26:8086"),
			};
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
		//[Fact]
		public void Rest_Get()
		{
			var task = _client.GetAsync("t1/r1/f1:c1");
			var res = task.Result;
			var content = res.Content.ReadAsStringAsync().Result;
			var model = JsonConvert.DeserializeObject<Table>(content);
		}

		[Fact]
		public void Rest_Post_CreateTable()
		{
			var tableName = "TableFromSinxUnitTest" + _getNowDateTime();
			var columnFamily = "cf" + _getNowDateTime();
			var request = new HttpRequestMessage(HttpMethod.Post, $"/{tableName}/schema");
			request.Headers.Add("Accept", "text/xml");
			request.Content = new StringContent(
				$"<?xml version=\"1.0\" encoding=\"UTF-8\"?><TableSchema name=\"{tableName}\">" +
				$"<ColumnSchema name=\"{columnFamily}\" /></TableSchema>");
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
			var task = _client.SendAsync(request);
			var response = task.Result;
			Assert.Equal(response.StatusCode, HttpStatusCode.Created);
		}

		//[Theory]
		//[InlineData("t1", "r1", "f1:c1")]
		public void Rest_Put_InsertTableRow(string tableName, string row, string column)
		{
			var value = _getNowDateTime(); //Convert.ToBase64String(Encoding.UTF8.GetBytes(_getNowDateTime()));
			var request = new HttpRequestMessage(HttpMethod.Put, $"/{tableName}/{row}");
			request.Headers.Add("Accept", "application/json");
			request.Content = new StringContent($"{{\"Row\":[{{\"key\":\"{_toBase64(row)}\", \"Cell\": [{{\"column\":\"{_toBase64(column)}\", \"$\":\"{_toBase64(value)}\"}}]}}]}}");
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var task = _client.SendAsync(request);
			var response = task.Result;
			Assert.Equal(response.StatusCode, HttpStatusCode.OK);
		}

		[Fact]
		public void Rest_Put_UpdateTableSchema()
		{
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
