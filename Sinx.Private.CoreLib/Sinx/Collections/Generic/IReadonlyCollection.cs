using System.Collections;
using System.Collections.Generic;

namespace Sinx.Collections.Generic
{
	public interface IReadonlyCollection<out T> : IEnumerable<T>
	{
		int Count { get; }
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}