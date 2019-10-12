using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Reactive.Tests
{
    /// <summary>
    /// 实现了ISafeObserver, ObserverBase
    /// </summary>
    public class AutoDetachObserverTests
    {
        [Fact]
        public void AutoDetachObserver_ImplementISafeObserver() 
        {
            var myObserver = Observer.Create<int>(i =>
            {
                i.Dump();
                if (i == 1)
                {
                    throw new Exception("triggered by observer");
                }
            }, ex => ex.Message.ToString(), () => "completed".Dump());
            var observer = new AutoDetachObserver<int>(myObserver);
            observer.SetResource(Disposable.Create(() => "obsrever exception i will clearnup by IDisposable Resource".Dump()));
            var observable = new MyObservable();
            observable.Subscribe(observer);
        }

        public class AutoDetachObserver<T> : ObserverBase<T>, ISafeObserver<T>
        {
            private readonly IObserver<T> _observer;
            private IDisposable _disposable;
            private bool _isDisposabled = false;

            public AutoDetachObserver(IObserver<T> observer)
            {
                _observer = observer;
            }

            // 如果原来没设置IDisposable, 设置上
            // 如果原来设置了IDisposable
            // 		1. 如果已经Dispose了, 抛出异常
            // 		2. 如果没有Dispose, 换新的并将旧的Dispose了
            public void SetResource(IDisposable resource)
            {
                var old = Interlocked.Exchange(ref _disposable, resource);
                if (old != null)
                {
                    if (_isDisposabled)
                    {
                        throw new Exception("DISPOSABLE_ALREADY_ASSIGNED");
                    }
                    _disposable?.Dispose();
                    _isDisposabled = false;
                }
                else
                {
                    _isDisposabled = true;
                }
            }

            protected override void OnCompletedCore()
            {
                try
                {
                    _observer.OnCompleted();
                }
                finally
                {
                    Dispose();
                }
            }

            // NOTICE: 异常还是抛出了
            protected override void OnErrorCore(Exception error)
            {
                try
                {
                    _observer.OnError(error);
                }
                finally
                {
                    Dispose();
                }
            }

            protected override void OnNextCore(T value)
            {
                var isNoError = false;
                try
                {
                    _observer.OnNext(value);
                    isNoError = true;
                }
                finally
                {
                    if (!isNoError)
                    {
                        Dispose();
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (_isDisposabled)
                    {
                        _disposable.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Base interface for observers that can dispose of a resource on a terminal notification
        /// or when disposed itself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal interface ISafeObserver<in T> : IObserver<T>, IDisposable
        {
            void SetResource(IDisposable resource);
        }

        public class MyObservable : IObservable<int>
        {
            public IDisposable Subscribe(IObserver<int> observer)
            {
                foreach (var num in Enumerable.Range(0, 10))
                {
                    if (num == 2)
                    {
                        observer.OnError(new Exception("observer OnError triggered"));
                    }
                    // 在2出现异常以后, observer已经停止处理, 3不会显示
                    else if (num == 4)
                    {
                        throw new Exception("Subscribe exception triggered");
                    }
                    else
                    {
                        try
                        {
                            observer.OnNext(num);
                        }
                        catch (Exception ex)
                        {
                            $"from subscribe {ex.Message}".Dump();
                            throw;
                        }
                    }
                }
                observer.OnCompleted();
                return Task.CompletedTask;
            }
        }
    }
}
