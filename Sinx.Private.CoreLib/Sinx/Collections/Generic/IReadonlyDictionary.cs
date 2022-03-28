using System.Collections.Generic;

namespace Sinx.Collections.Generic
{
	public interface IReadonlyDictionary<TKey, TValue> : IReadonlyCollection<KeyValuePair<TKey, TValue>>
	{
		TValue this[TKey key] { get; }
		IEnumerable<TKey> Keys { get; }
		IEnumerable<TValue> Values { get; }

		bool ContainsKey(TKey key);

		bool TryGetValue(TKey key, out TValue value);
	}
}