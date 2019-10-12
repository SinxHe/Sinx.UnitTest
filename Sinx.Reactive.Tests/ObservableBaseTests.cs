using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Reactive.Tests
{
    public class ObservableBaseTests
    {
        [Fact]
        public async Task Run() 
        {
            var xs = Observable.Interval(TimeSpan.FromSeconds(1)).Dump();
            var ys = Observable.Create<int>(o =>
            {
                for (int i = 0; i < 10; i++)
                {
                    o.OnNext(i);
                    Task.Delay(1000).Wait();
                }
                return Disposable.Empty;
            });
            var res = xs.CombineLatest(ys, (x, y) =>
            {
                $"{x}-{y}".Dump();
                if (x == 3)
                {
                    throw new Exception();
                }
                return x + y;
            });
            await res.Dump();
        }
    }
}
