using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Concurrency.Tests._01_同步上下文
{
    public class SynchronizationContextTests
    {
	    [Fact]
        public async Task Concurrency_SynchronizationContext()
        {
	        var context = SynchronizationContext.Current;
			var scheduler = TaskScheduler.Current;

	        Assert.NotNull(context);
	        Assert.NotNull(scheduler);

	        var thread = new Thread(() =>
	        {
				context = SynchronizationContext.Current;
				scheduler = TaskScheduler.Current;
	        });
	        thread.Start();
	        thread.Join();
	        Assert.Null(context);
	        Assert.NotNull(scheduler);


	        Task.Run(() =>
	        {
				// 这个里面使用线程池线程进行的处理
		        context = SynchronizationContext.Current;
		        scheduler = TaskScheduler.Current;
	        }).Wait();
			// 由于上面是Wait等待, 所以这里一定是使用的调用线程进行的执行
	        Assert.NotNull(SynchronizationContext.Current);
	        Assert.Null(context);
	        Assert.NotNull(scheduler);

	        await Task.Run(() =>
	        {
				// NOTICE: 这里使用的是线程池线程进行的执行
				context = SynchronizationContext.Current;
				scheduler = TaskScheduler.Current;
	        });
			// NOTICE: 虽然上面进行了await等待, 但是异步会在[await]等待
			// NOTICE: 以后捕捉上下文, 保证下面的同步块使用的是调用线程进行的执行
			// NOTICE: 就好像是使用了[.Wait()]等待一样
	        Assert.NotNull(SynchronizationContext.Current);
	        Assert.Null(context);
	        Assert.NotNull(scheduler);

	        await Task.Run(() =>
	        {
		        context = SynchronizationContext.Current;
		        scheduler = TaskScheduler.Current;
	        }).ConfigureAwait(false);
	        Assert.Null(SynchronizationContext.Current);	// NOTICE
	        Assert.Null(context);
	        Assert.NotNull(scheduler);
        }

	    [Fact]
	    public void Concurrency_SynchronizationContext_Send()
	    {
		    var context = new SynchronizationContext();
			var id = Thread.CurrentThread.ManagedThreadId;
			Console.WriteLine($"Test Func Thread Id: {id}");
		    context.Send(e =>
		    {
				var id2 = Thread.CurrentThread.ManagedThreadId;
				Console.WriteLine($"The Action Delivery To [Send] Method Invoke Thread Id: {id2}");
		    }, null);

		    context.Post(e =>
		    {
			    var id2 = Thread.CurrentThread.ManagedThreadId;
			    Console.WriteLine($"The Action Delivery To [Post] Method Invoke Thread Id: {id2}");
		    }, null);
			// 等待Post完成
		    Thread.Sleep(1000);
	    }
    }
}
