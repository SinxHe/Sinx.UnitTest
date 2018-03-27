using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace Sinx.Concurrency.Tests._03_Rx
{
    public class _01_IObservable创建
    {
        /// <summary>
        /// Empty -> OnComplete
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void IObservable_Empty()
        {
            var observable = Observable.Empty<int>();   // 冷启动
            var dis = observable.Subscribe(Console.WriteLine, () => Console.WriteLine("Complete"));
            dis.Dispose();  // 取消订阅
        }

        /// <summary>
        /// Return -> OnNext -> OnComplete
        /// </summary>
        [Fact]
        public void IObservable_Return()
        {
            var observable = Observable.Return(1);
            var dis = observable.Subscribe(Console.WriteLine, () => Console.WriteLine("Complete"));
            dis.Dispose();
        }

        [Fact]
        public async Task IObservable_Create_Create()
        {
            // 没有输出
            var observable = Observable.Create<int>(async o =>
            {
                foreach (var item in Enumerable.Range(0, 10))
                {
                    var t = await Task.FromResult(item);
                    Console.WriteLine($"Create: {t}");
                    o.OnNext(t);
                }
            });
            // 没有输出
            IObservable<IList<int>> observableToList = observable.ToList();
            // 进行等待, 然后有输出
            await observableToList;
            // 再调用一次, 又输出一次
            var observableList = await observableToList;
            foreach (var item in observableList)
            {
                Console.WriteLine($"Use: {item}");
            }
            // 边生成, 边使用
            await observable.ForEachAsync(i => Console.WriteLine($"ForEachAsync: {i}"));
        }

        [Fact]
        public async Task IObservable_Create_Range_UseCommonLinq()
        {
            IObservable<int> observable0 = Observable.Range(0, 10);
            IObservable<string> observable1 = observable0.Where(i => i % 2 == 0)
                .Select(i => i.ToString() + i.ToString());
            // Obsolete
            observable1.ForEach(i => Console.WriteLine($"ForEach: {i}"));
            await observable1.ForEachAsync(i => Console.WriteLine($"ForEachAsync: {i}"));
        }

        [Fact]
        public async Task IObservable_Create_Interval()
        {
            IObservable<IList<long>> observable = Observable.Interval(TimeSpan.FromSeconds(1))
                .Buffer(2);
            IDisposable dis = observable.Subscribe(e =>
                    Console.WriteLine(DateTime.Now.Second +
                                    $": Got {e[0]} and {e[1]}"));
            Console.WriteLine("Not Finished");
            //dis.Dispose(); // dispose以后就不会显示了
            await observable;
            Console.WriteLine("Finished");
        }

        /// <summary>
        /// 从标准事件模式创建, 对于非标准的可以使用FromEvent
        /// </summary>
        /// <remarks>
        /// 标准模式事件: 第一个参数是发送者, 第二个是参数
        /// </remarks>
        [Fact]
        public void IObservable_Create_FromEventPattern()
        {
            // 新版EventHandler<T>
            var progress = new Progress<int>();
            var progressReports = Observable.FromEventPattern<int>(
                handler => progress.ProgressChanged += handler,
                handler => progress.ProgressChanged -= handler);
            // 这里的EventArgs是强类型int
            progressReports.Subscribe(data => Console.WriteLine($"OnNext: {data.EventArgs}"));

            // 旧版为每个事件定义一个委托类型
            var timer = new System.Timers.Timer(100) { Enabled = true };
            var ticks1 = Observable.FromEventPattern<ElapsedEventHandler, ElapsedEventArgs>(
                // 转换器, EventHandler<ElapsedEventArgs> => ElapsedEventHandler
                handler => (s, a) => handler(s, a),
                handler => timer.Elapsed += handler,
                handler => timer.Elapsed -= handler);
            // EventArgs依旧是强类型, FromEventPattern的类型参数是对应的时间处理程序和EventArgs的派生类
            ticks1.Subscribe(data => Console.WriteLine("ticks1 OnNext: " + data.EventArgs.SignalTime));
            // ------ 使用反射 -------
            var ticks2 = Observable.FromEventPattern(timer, nameof(timer.Elapsed));
            ticks2.Subscribe(data => Console.WriteLine("ticks2 OnNext: " + ((ElapsedEventArgs)data.EventArgs).SignalTime));

            Thread.Sleep(1000);
        }

        /// <summary>
        /// 针对带有异常的AsyncCompletedEventArgs.Error事件, Rx会把这个作为一个数据事件
        /// </summary>
        [Fact]
        public void IObservable_FromEventPattern_AsyncCompletedEventArgs_ErrorHandl()
        {
            var client = new WebClient();
            var downloadedString = Observable.FromEventPattern(client, nameof(client.DownloadStringCompleted));
            downloadedString.Subscribe(data =>
            {
                var eventArgs = (DownloadStringCompletedEventArgs)data.EventArgs;
                if (eventArgs.Error != null)
                {
                    Console.WriteLine("OnNext:(Error) " + eventArgs.Error);
                }
                else
                {
                    Console.WriteLine("OnNext: " + eventArgs.Result);
                }
            },
                ex => Console.WriteLine("OnError: " + ex.ToString()),
                () => Console.WriteLine("OnCompleted"));
            client.DownloadStringAsync(new Uri("http://baidu.com"));
            Thread.Sleep(3000);
            client.DownloadStringAsync(new Uri("http://invalid.example.com"));
            Thread.Sleep(3000);
        }
    }
}
