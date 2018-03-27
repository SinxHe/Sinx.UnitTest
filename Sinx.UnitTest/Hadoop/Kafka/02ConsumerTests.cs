using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		private const string TOPIC_HAS_TWO_VALUE = "topic-has-two-value";
		private const string TOPIC_A_LARGE_COUNT = "a_large_topic";

		[Fact]
		public void Consumer_ConsumeALargeTopic_MutipleProcessThread()
		{
			// Arrange
			var groupName = "just4consumer-a-large-topic";
			var consumer = GetConsumer(groupName, false);

			// Act
			consumer.OnPartitionEOF += (_, end) =>
			{
			};
			consumer.OnMessage += (_, msg) =>
			{
				((Consumer<Null, string>)_).CommitAsync();
				Console.WriteLine($"OnMessage: CommitAsync: {msg.Offset}");
			};

			consumer.Subscribe(TOPIC_A_LARGE_COUNT);

			for (int i = 0; i < 10; i++)
			{
				Console.WriteLine("----------poll----------");
				consumer.Poll(TimeSpan.FromSeconds(5));
			}
			// Assert

		}

		private int _countLimit;
		[Fact]
		public void Consumer_OnMessage_CountLimitOnMutipleThread()
		{
			var ts = TimeSpan.FromSeconds(5);
			var consumer = GetConsumer();
			consumer.OnMessage += (s, e) =>
			{
				Interlocked.Increment(ref _countLimit);
				Task.Run(() =>
				{
					Task.Delay(ts).Wait();
					Interlocked.Decrement(ref _countLimit);
				});
			};
			consumer.Assign(GetTopicPartitionOffset(TOPIC_A_LARGE_COUNT));
			while (true)
			{
				if (_countLimit < 100)
				{
					consumer.Poll(ts);
				}
				else
				{
					break;
				}
			}

			consumer.Dispose();
		}

		[Fact]
		public void Consumer_Commit_CommitHadCommitedOffset()
		{
			// Arrange
			var consumer = GetConsumer("fdsa", false);
			consumer.OnMessage += (s, e) =>
			{
				var c = (Consumer<Null, string>)s;
				if (e.Offset == 10)
				{
					// 下一次单元测试拿到的Offset是3
					c.CommitAsync(new[] { new TopicPartitionOffset(e.Topic, 0, 3) });
				}
				else
				{
					c.CommitAsync(e);
					c.Poll(TimeSpan.FromSeconds(10));
				}
			};
			consumer.Subscribe(TOPIC_A_LARGE_COUNT);

			// Act
			consumer.Poll(TimeSpan.FromSeconds(10));
			consumer.Poll(TimeSpan.FromSeconds(10));

			// Assert
			consumer.Dispose();
		}

		/// <summary>
		/// 按照顺序Commit, 能够正确的拿到最后Commit后的下一个数据
		/// </summary>
		[Fact]
		public void Consumer_Commit_OrderCommit()
		{
			// Arrange
			var consumer = GetConsumer(autoCommit: false);
			consumer.OnMessage += (s, e) =>
			{
				((Consumer<Null, string>)s).CommitAsync(e);
				Console.WriteLine($"OnMessage: CommitAsync: {e.Offset}");
			};
			consumer.Subscribe(TOPIC_A_LARGE_COUNT);
			var commitedFormatString = string.Join(",", consumer.Committed(
				new[] { new TopicPartition(TOPIC_A_LARGE_COUNT, 0) },
				TimeSpan.FromSeconds(10)).Select(e => e.Offset));
			Console.WriteLine($"Commited: {commitedFormatString}");

			// Act
			consumer.Poll(TimeSpan.FromSeconds(10));
			consumer.Poll(TimeSpan.FromSeconds(10));
			consumer.Poll(TimeSpan.FromSeconds(10));

			consumer.Dispose();
		}

		[Fact]
		public void Consumer_OnMessage_RaisedByPoll()
		{
			// Arrange
			var consumer = GetConsumer();
			var onMsgCount = 0;
			consumer.OnMessage += (_, msg) =>
			{
				onMsgCount++;
			};
			consumer.Assign(GetTopicPartitionOffset(TOPIC_HAS_TWO_VALUE));
			var msgCountTrace = new List<int>();

			// Act
			consumer.Poll(TimeSpan.FromSeconds(50));
			msgCountTrace.Add(onMsgCount);
			consumer.Poll(TimeSpan.FromSeconds(50));
			msgCountTrace.Add(onMsgCount);

			// Assert
			Assert.Equal(1, msgCountTrace.ToArray()[0]);
			Assert.Equal(2, msgCountTrace.ToArray()[1]);
		}

		[Fact]
		public void Consumer_OnPartitionEOF_ConsumeToEnd()
		{
			// Arrange
			var consumer = GetConsumer();
			consumer.Assign(GetTopicPartitionOffset(TOPIC_HAS_TWO_VALUE));
			var isEofRaised = false;
			consumer.OnPartitionEOF += (s, a) => isEofRaised = true;

			// Act
			// - OnMessage: consume first data
			consumer.Poll(TimeSpan.FromSeconds(10));
			// - OnMessage: consume second data
			consumer.Poll(TimeSpan.FromSeconds(10));
			var temp = isEofRaised;
			// - OnPartitionEOF
			consumer.Poll(TimeSpan.FromSeconds(10));

			// Assert
			Assert.True(isEofRaised);
			Assert.False(temp);
		}

		[Fact]
		public void Consumer_OnPartitionEOF_TwoEmptyTopic()
		{
			// Arrange
			var groupName = "test-4-empty-partition";
			var consumer = GetConsumer(groupName);
			var tpo = GetTopicPartitionOffset("not_exist_topic0", "not_exist_topic1");
			var isEmpty = false;
			var isEOF = false;

			// Act
			// 当消费到分区结尾的时候触发, 即使是空队列
			consumer.OnPartitionEOF += (_, end) => { isEmpty = true; isEOF = true; };
			consumer.Assign(tpo);
			consumer.Poll(TimeSpan.FromSeconds(10));

			// Assert
			Assert.True(isEmpty);
			Assert.True(isEOF);
		}

		/// <summary>
		/// 订阅的才会在Subscription中看到Topic, 赋值的不会
		/// NOTICE: 订阅的有动态调整的能力
		/// </summary>
		[Fact]
		public void Consumer_SubscribeAndAssign_SubscriptionValueAndAssignment()
		{
			// Arrange
			var consumer0 = GetConsumer();
			var consumer1 = GetConsumer();

			// Act
			var topics = new[] { "topic" };
			var tofs = GetTopicPartitionOffset("topic");
			consumer0.Subscribe(topics);
			consumer1.Assign(tofs);

			// Assert
			Assert.Equal(topics, consumer0.Subscription);
			Assert.Empty(consumer0.Assignment);


			Assert.Empty(consumer1.Subscription);
			Assert.Equal(tofs.Select(e => e.Topic), consumer1.Assignment.Select(e => e.Topic));
		}

		[Fact]
		public void Consumer_SimpleUsage()
		{
			// Arrange
			var groupName = "for-simple-usage";
			var consumer = GetConsumer(groupName);
			consumer.Assign(new[] { new TopicPartitionOffset("heshixiongtest", 0, 0) });
			// ts 指的是等待多长时间没有数据就执行下一条语句(不抛出异常)
			var ts = TimeSpan.FromSeconds(1);

			// Act
			var first = consumer.Consume(out var msgFirst, ts);
			var has0 = consumer.Consume(out var msg0, ts);
			var has1 = consumer.Consume(out var msg1, ts);
			var has2 = consumer.Consume(out var msg2, ts);

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

		private List<TopicPartitionOffset> GetTopicPartitionOffset(params string[] topics)
		{
			return topics.Select(e => new TopicPartitionOffset(e, 0, 0)).ToList();
		}

		private Consumer<Null, string> GetConsumer(
			string groupName = "default-group-name",
			bool autoCommit = true)
		{
			var conf = new ConfigurationBuilder()
				.AddJsonFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "sensitive.json"), false)
				.Build();
			var brokerList = conf["Kafka:Server:Hosts:0"];

			var advancedConsumerConfig = new Dictionary<string, object>
			{
				["group.id"] = groupName,
				// 自动后台提交offset
				["enable.auto.commit"] = autoCommit,
				["auto.commit.interval.ms"] = 5000,
				["statistics.interval.ms"] = 60000,
				["bootstrap.servers"] = brokerList,
				["default.topic.config"] = new Dictionary<string, object>
				{
					// earliest
					["auto.offset.reset"] = "earliest"
				}
			};
			var valDeserializer = new StringDeserializer(Encoding.UTF8);
			var consumer = new Consumer<Null, string>(advancedConsumerConfig, null, valDeserializer);
			ConfigureEvent(consumer);
			return consumer;
		}

		private void ConfigureEvent(Consumer<Null, string> consumer)
		{
			// NOTICE: All event handlers are called on the main thread.
			// Raised on deserialization errors or when a consumed message has an error != NoError.
			consumer.OnConsumeError += (_, msg)
				=> Console.WriteLine($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");
			// Raised on critical errors, e.g. connection failures or all brokers down.
			consumer.OnError += (_, error) => Console.WriteLine($"Error: {error}");
			consumer.OnLog += _advancedConsumer_OnLog;
			consumer.OnMessage += _advancedConsumer_OnMessage;
			consumer.OnOffsetsCommitted += _advancedConsumer_OnOffsetsCommitted;
			// 需要Poll触发
			consumer.OnPartitionEOF += _advancedConsumer_OnPartitionEOF;
			consumer.OnPartitionsAssigned += Consumer_OnPartitionsAssigned;
			consumer.OnPartitionsRevoked += Consumer_OnPartitionsRevoked;
			consumer.OnStatistics += (_, json) => Console.WriteLine($"Statistics: {json}");

			void Consumer_OnPartitionsAssigned(object sender, List<TopicPartition> partitions)
			{
				var c = (Consumer)sender;
				Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
				c.Assign(partitions);
			}

			void Consumer_OnPartitionsRevoked(object sender, List<TopicPartition> partitions)
			{
				var c = (Consumer)sender;
				Console.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]");
				try
				{
					c.Unassign();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"OnPartitionsRevoked: {ex.Message}");
				}
			}

			void _advancedConsumer_OnPartitionEOF(object sender, TopicPartitionOffset e)
			{
				Console.WriteLine($"OnPartitionEOF: [topic: {e.Topic} partition: {e.Partition} offset: {e.Offset}]");
			}

			void _advancedConsumer_OnOffsetsCommitted(object sender, CommittedOffsets commit)
			{
				if (commit.Error)
				{
					Console.WriteLine($"OnOffsetsCommitted: Failed to commit offsets: {commit.Error}");
				}
				Console.WriteLine($"OnOffsetsCommitted: Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
			}

			void _advancedConsumer_OnMessage(object sender, Message<Null, string> e)
			{
				Console.WriteLine(
					$"OnMessage: [topic: {e.Topic} partition: {e.Partition} " +
					$"offset: {e.Offset}] key: {e.Key} value: {e.Value}");
			}

			void _advancedConsumer_OnLog(object sender, LogMessage e)
			{
				Console.WriteLine($"OnLog: {JObject.FromObject(e)}");
			}
		}
	}
}