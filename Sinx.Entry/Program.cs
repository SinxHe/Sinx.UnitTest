using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sinx.Entry
{
    public static class Program
    {
        public static void Main()
        { 
	        var context = SynchronizationContext.Current;
	        var scheduler = TaskScheduler.Current;
	        Debug.Assert(context != null);
	        Debug.Assert(scheduler == null);
        }
    }
}
