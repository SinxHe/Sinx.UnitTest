using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Sinx.UnitTest.Hadoop.Kafka
{
	public class _01PublisherTests
	{
		[Fact]
		public async Task Publisher_SummaryAsync()
		{
			var conf = new ConfigurationBuilder()
				.AddJsonFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "sensitive.json"), false)
				.Build();
			var brokerList = conf["Kafka:Server:Hosts:0"];
			var topicName = "heshixiongtest";

			var config = new Dictionary<string, object> { { "bootstrap.servers", brokerList } };

			using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
			{
				for (int i = 0; i < 2; i++)
				{
					var res = await producer.ProduceAsync(topicName, null, $"value{DateTime.Now}");
					Console.WriteLine(res.Partition);
					Console.WriteLine(res.Offset);
				}
				// Tasks are not waited on synchronously (ContinueWith is not synchronous),
				// so it's possible they may still in progress here.
				producer.Flush(TimeSpan.FromSeconds(10));
			}
		}
	}
}