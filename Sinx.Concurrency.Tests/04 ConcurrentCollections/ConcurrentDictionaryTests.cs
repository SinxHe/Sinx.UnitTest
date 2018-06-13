using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Concurrency.Tests._04_ConcurrentCollections
{
    public class ConcurrentDictionaryTests
    {
        private readonly ILogger _logger;
        public ConcurrentDictionaryTests()
        {
            var logger = new LoggerFactory().AddDebug().CreateLogger<ConcurrentDictionaryTests>();
            _logger = logger;
        }

        [Fact]
        public void ConcurrentDictionary_CanDoTheDictionaryDo()
        {
            // Arrange
            var dic = new Dictionary<int, int>();
            IDictionary conDic = new ConcurrentDictionary<int, int>();

            // Act
            // - Add
            dic.Add(1, 1);
            conDic.Add(1, 1);
            dic[2] = 2;
            conDic[2] = 2;

            // Assert
            Assert.NotEmpty(dic);
            Assert.Equal(dic, conDic);

            // Note
            // 在ConcurrentDictionary中Add和Remove是private的, 所以要用接口访问, 这里为什么要隐藏呢?
            // 因为Dic中Key值是唯一的, 所以使用Add假设你知道Dic中没有这个键值对
            // 而在ConDic中, 因为面对的是多线程场景, 所以Add的假设是无法保证的
        }

        [Fact]
        public void ConcurrentDictionary_TryUpdate_NeedSpecifyOldValue()
        {
            // Arrange
            var conDic = new ConcurrentDictionary<int, int>();
            conDic.TryAdd(1, 1);
            var listIsUpdate = new List<bool>();

            // Act
            Enumerable
                .Range(0, 10)
                .AsParallel()
                .WithDegreeOfParallelism(5)
                .ForAll(i =>
                {
                    // 需要指定旧的值, 如果旧值一样,
                    // 执行, 否则, 跳过
                    var isUpdate = conDic.TryUpdate(1, 2, 1);
                    listIsUpdate.Add(isUpdate);
                });

            // Act
            Assert.Equal(2, conDic[1]);
            Assert.True(listIsUpdate.Count > 0);
            Assert.Single(listIsUpdate.Where(e => e));
        }

        [Fact]
        public async Task ConcurrentDictionary_AddOrUpdate_ForIncrementSituation()
        {
            // Arrange
            var dic = new Dictionary<string, int>
            {
                [1.ToString()] = 1
            };
            var dic1 = new Dictionary<string, int>
            {
                [1.ToString()] = 1
            };
            var conDic0 = new ConcurrentDictionary<string, int>();
            var conDic1 = new ConcurrentDictionary<string, int>();
            var invokeTimes = 0;

            // Act
            var tasks = Enumerable
                .Range(0, 1000)
                .Select(async e =>
                {
                    await Task.Delay(1000);
                    _logger.LogInformation(e.ToString());
                    dic[1.ToString()]++;
                    dic1[1.ToString()] = dic1[1.ToString()] + 1;
                    conDic0.AddOrUpdate(1.ToString(), 1, (k, v) => v + 1);
                    conDic1.AddOrUpdate(1.ToString(), 1, (k, v) =>
                    {
                        Interlocked.Increment(ref invokeTimes);
                        return v + 1;
                    });
                });
            await Task.WhenAll(tasks);

            // Assert
            // TODO - BEGIN
            Assert.Equal(1001, dic[1.ToString()]);
            Assert.Equal(1001, dic1[1.ToString()]);
            // TODO - END
            Assert.Equal(1000, conDic0[1.ToString()]);
            Assert.Equal(1000, conDic1[1.ToString()]);
            Assert.NotEqual(1000, invokeTimes);
        }
    }
}
