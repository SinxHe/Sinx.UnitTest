using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Sinx.UnitTest.Hadoop.Kafka
{
	/// <summary>
	/// NOTICE: 所有事件都是在主线程上进行的
	/// </summary>
	public class _02ConsumerTests
	{
		private readonly Consumer<Ignore, string> _simpleConsumer;

		private readonly Consumer<Null, string> _advancedConsumer;

		public _02ConsumerTests()
		{
			var conf = new ConfigurationBuilder()
				.AddJsonFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "sensitive.json"), false)
				.Build();
			var brokerList = conf["Kafka:Server:Hosts:0"];

			var config = new Dictionary<string, object>
			{
				{"group.id", "simple-csharp-consumer"},
				{"bootstrap.servers", brokerList}
			};
			_simpleConsumer = new Consumer<Ignore, string>(config, null, new StringDeserializer(Encoding.UTF8));
			_simpleConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset("heshixiongtest", 0, 0) });

			var advancedConsumerConfig = new Dictionary<string, object>
			{
				["group.id"] = "advanced-consumer-tests",
				["enable.auto.commit"] = true,
				["auto.commit.interval.ms"] = 5000,
				["statistics.interval.ms"] = 60000,
				["bootstrap.servers"] = brokerList,
				["default.topic.config"] = new Dictionary<string, object>
				{
					// earliest
					["auto.offset.reset"] = "beginning"
				}
			};

			var valDeserializer = new StringDeserializer(Encoding.UTF8);
			_advancedConsumer = new Consumer<Null, string>(advancedConsumerConfig, null, valDeserializer);
			// NOTICE: All event handlers are called on the main thread.

			// Raised on deserialization errors or when a consumed message has an error != NoError.
			_advancedConsumer.OnConsumeError += (_, msg)
				=> Console.WriteLine($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");

			// Raised on critical errors, e.g. connection failures or all brokers down.
			_advancedConsumer.OnError += (_, error)
				=> Console.WriteLine($"Error: {error}");

			_advancedConsumer.OnLog += _advancedConsumer_OnLog;

			_advancedConsumer.OnMessage += _advancedConsumer_OnMessage;

			_advancedConsumer.OnOffsetsCommitted += _advancedConsumer_OnOffsetsCommitted;

			_advancedConsumer.OnPartitionEOF += _advancedConsumer_OnPartitionEOF;

			_advancedConsumer.OnPartitionsAssigned += (_, partitions) =>
			{
				Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {_advancedConsumer.MemberId}");
				_advancedConsumer.Assign(partitions);
			};

			_advancedConsumer.OnPartitionsRevoked += (_, partitions) =>
			{
				Console.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]");
				_advancedConsumer.Unassign();
			};

			_advancedConsumer.OnStatistics += (_, json)
				=> Console.WriteLine($"Statistics: {json}");
		}

		private void _advancedConsumer_OnPartitionEOF(object sender, TopicPartitionOffset e)
		{
			Console.WriteLine($"OnPartitionEOF: {JObject.FromObject(e)}"); 
		}

		private void _advancedConsumer_OnOffsetsCommitted(object sender, CommittedOffsets commit)
		{
			Console.WriteLine($"[{string.Join(", ", commit.Offsets)}]");

			if (commit.Error)
			{
				Console.WriteLine($"Failed to commit offsets: {commit.Error}");
			}
			Console.WriteLine($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
		}

		private void _advancedConsumer_OnMessage(object sender, Message<Null, string> e)
		{
			Console.WriteLine($"OnMessage: {JObject.FromObject(e)}");
		}

		private void _advancedConsumer_OnLog(object sender, LogMessage e)
		{
			Console.WriteLine($"OnLog: {JObject.FromObject(e)}");
		}

		[Fact]
		public void Consumer_SimpleUsage()
		{
			// Arrange
			// ts 指的是等待多长时间没有数据就执行下一条语句(不抛出异常)
			var ts = TimeSpan.FromSeconds(1);

			// Act
			var first = _simpleConsumer.Consume(out var msgFirst, ts);
			var has0 = _simpleConsumer.Consume(out var msg0, ts);
			var has1 = _simpleConsumer.Consume(out var msg1, ts);
			var has2 = _simpleConsumer.Consume(out var msg2, ts);

			// Assert
			Assert.False(first);
			Assert.True(has0);
			Assert.True(has1);
			Assert.False(has2);

			Assert.Null(msgFirst);
			Assert.NotNull(msg0);

			Assert.Equal("heshixiongtest", msg1.Topic);
			Assert.Null(msg1.Key);
			Assert.NotNull(msg1.Value);
			// Offset 从0开始
			Assert.Equal(1, msg1.Offset);
			// Partion 默认是0
			Assert.Equal(0, msg1.Partition);

			Assert.Null(msg2);
		}

		[Fact]
		public void Consumer_EmptyPartition()
		{
			// Arrange
			var topics = new[]
			{
				"not_exist_topic0",
				"not_exist_topic1"
			};
			var isEmpty = false;
			var isEOF = false;

			// Act
			// 当消费到分区结尾的时候触发, 即使是空队列
			_advancedConsumer.OnPartitionEOF += (_, end)
				=>
			{
				isEmpty = true;
				isEOF = true;
			};
			// NOTICE: Subscibe 方法要写在所有准备工作, 事件委托配置以后
			_advancedConsumer.Subscribe(topics);
			
			// 一直阻塞线程到有Event触发或者指定的时间片到期
			// 通常应该写入一个小的时间片因为这个操作不能取消
			// 主线程就是从这里进入拿到的执行事件的机会
			_advancedConsumer.Poll(TimeSpan.FromSeconds(10));
			
			// Assert
			Assert.Equal(topics, _advancedConsumer.Subscription);
			// 是在主线程上进行的Event执行
			
			Assert.True(isEmpty);
			Assert.True(isEOF);
			// 这里需要手动Dispose, 不然OnPartitionsAssigned会出现异常
			_advancedConsumer.Dispose();
		}

		[Fact]
		public void Consumer_ConsumeToEnd()
		{
			// Arrange
			var topics = new[] { "the_topic_hold_2_message_for_consume_to_end" };
			var eofCount = 0;
			var onMsgCount = 0;

			// Act
			_advancedConsumer.OnPartitionEOF += (_, end) =>
			{
				eofCount++;
				_advancedConsumer.Poll(TimeSpan.FromSeconds(5));
			};
			_advancedConsumer.OnMessage += (_, msg) =>
			{
				onMsgCount++;
				_advancedConsumer.Poll(TimeSpan.FromSeconds(5));
			};
			var topicPartitionOffsets = topics.Select(e => new TopicPartitionOffset(e, 0, 0));
			_advancedConsumer.Assign(topicPartitionOffsets);
			_advancedConsumer.Poll(TimeSpan.FromSeconds(5));
			// Assert
			Assert.Equal(1, eofCount);
			Assert.Equal(2, onMsgCount);
		}

		[Fact]
		public void Consumer_Subscribe()
		{
			// Arrange
			var topics = new[] { "the_topic_hold_2_message_for_consume_to_end" };
			var eofCount = 0;
			var onMsgCount = 0;

			// Act
			_advancedConsumer.OnPartitionEOF += (_, end) =>
			{
				eofCount++;
				_advancedConsumer.Poll(TimeSpan.FromSeconds(5));
			};
			_advancedConsumer.OnMessage += (_, msg) =>
			{
				onMsgCount++;
				_advancedConsumer.Poll(TimeSpan.FromSeconds(5));
			};
			// TODO: 为什么使用订阅的方式拿不到数据呢????
			_advancedConsumer.Subscribe(topics);
			_advancedConsumer.Poll(TimeSpan.FromSeconds(5));

			// Assert
			Assert.Equal(1, eofCount);
			Assert.Equal(2, onMsgCount);
		}
	}
}