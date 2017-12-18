using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Sinx.UnitTest.Hadoop.Kafka
{
	public class _02ConsumerTests
	{
		public _02ConsumerTests()
		{

		}

		[Fact]
		public async Task Consumer_SummaryAsync()
		{
			var conf = new ConfigurationBuilder()
				.AddJsonFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "sensitive.json"), false)
				.Build();
			var brokerList = conf["Kafka:Server:Hosts:0"];
			var topics = new[] { "heshixiongtest" };

			var config = new Dictionary<string, object>
			{
				{"group.id", "simple-csharp-consumer"},
				{"bootstrap.servers", brokerList}
			};

			using (var consumer = new Consumer<Ignore, string>(config, null, new StringDeserializer(Encoding.UTF8)))
			{
				consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topics.First(), 0, 0) });

				// Raised on critical errors, e.g. connection failures or all brokers down.
				consumer.OnError += (_, error)
					=> Console.WriteLine($"Error: {error}");

				// Raised on deserialization errors or when a consumed message has an error != NoError.
				consumer.OnConsumeError += (_, error)
					=> Console.WriteLine($"Consume error: {error}");

				var count = 0;
				while (true)
				{
					var isConsume = consumer.Consume(out var msg, TimeSpan.FromSeconds(1));
					if (isConsume)
					{
						Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
					}
					else if(count++ > 10)
					{
						break;
					}
				}
			}
		}
	}
}