using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
		private readonly Producer<Null, string> _producer;
		public _01PublisherTests()
		{
			var conf = new ConfigurationBuilder()
				.AddJsonFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "sensitive.json"), false)
				.Build();
			var brokerList = conf["Kafka:Server:Hosts:0"];
			var config = new Dictionary<string, object> { { "bootstrap.servers", brokerList } };
			_producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
		}

		[Fact]
		public async Task Publisher_SimpleUsageAsync()
		{
			// Arrange
			var topicName = DateTime.Now.Ticks.ToString();

			// Act
			var res0 = await _producer.ProduceAsync(topicName, null, $"value{DateTime.Now}");
			var res1 = await _producer.ProduceAsync(topicName, null, $"value{DateTime.Now}");
			// 等待异步操作执行完成
			_producer.Flush(TimeSpan.FromSeconds(10));

			// Assert
			// 判断是否成功
			Assert.Equal(ErrorCode.NoError, res0.Error.Code);
			Assert.Equal(ErrorCode.NoError, res1.Error.Code);

			// 默认发布到分区0里面
			Assert.Equal(0, res0.Partition);
			// 空队列从0开始计数, 返回此消息放入的计数
			Assert.Equal(0, res0.Offset);
			// 位移自增因子是1
			Assert.Equal(1, res1.Offset);

			// 还会把键值对放进返回结构中
			Assert.Null(res0.Key);
			Assert.NotNull(res1.Value);
		}

		[Fact(Skip = "a_large_topic has coreated")]
		public void Publisher_GenerateABigPartition()
		{
			const string topicName = "a_large_topic";
			var i = 0;
			var beginTime = DateTime.Now;
			while (true)
			{
				_producer.ProduceAsync(topicName, null, $"value-{Interlocked.Increment(ref i)}");
				if (DateTime.Now - beginTime > TimeSpan.FromMinutes(60))
				{
					break;
				}
			}
		}
	}
}