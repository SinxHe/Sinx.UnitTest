using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.Concurrency.Tests._01_ͬ��������
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
				// �������ʹ���̳߳��߳̽��еĴ���
		        context = SynchronizationContext.Current;
		        scheduler = TaskScheduler.Current;
	        }).Wait();
			// ����������Wait�ȴ�, ��������һ����ʹ�õĵ����߳̽��е�ִ��
	        Assert.NotNull(SynchronizationContext.Current);
	        Assert.Null(context);
	        Assert.NotNull(scheduler);

	        await Task.Run(() =>
	        {
				// NOTICE: ����ʹ�õ����̳߳��߳̽��е�ִ��
				context = SynchronizationContext.Current;
				scheduler = TaskScheduler.Current;
	        });
			// NOTICE: ��Ȼ���������await�ȴ�, �����첽����[await]�ȴ�
			// NOTICE: �Ժ�׽������, ��֤�����ͬ����ʹ�õ��ǵ����߳̽��е�ִ��
			// NOTICE: �ͺ�����ʹ����[.Wait()]�ȴ�һ��
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
			// �ȴ�Post���
		    Thread.Sleep(1000);
	    }
    }
}
