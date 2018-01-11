using System;

namespace Sinx.Concurrency.Tests._01_同步上下文
{
	public interface ISinxSynchronizeInvoke
	{
		Action Delegate { get; set; }
	}

	public class SinxSynchronizeInvoke : ISinxSynchronizeInvoke
	{
		public Action Delegate { get; set; }
	}
}
