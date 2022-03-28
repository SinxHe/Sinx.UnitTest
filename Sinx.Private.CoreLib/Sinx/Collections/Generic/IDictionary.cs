using System.Collections.Generic;

namespace Sinx.Collections.Generic
{
	public interface IDictionary<TKey, TValue> : IReadonlyDictionary<TKey, TValue>,
		ICollection<KeyValuePair<TKey, TValue>>
	{
		new TValue this[TKey key] { get; set; }

		void Add(TKey key, TValue value);
		bool Remove(TKey key);

		TValue IReadonlyDictionary<TKey, TValue>.this[TKey key] => this[key];

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			if (ContainsKey(item.Key))
			{
				return;
			}
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			// EqualityComparer: null == null => true
			if (item.Value == null)
			{
				return false;
			}
			if (TryGetValue(item.Key, out var val))
			{
				return EqualityComparer<TValue>.Default.Equals(val, item.Value);
			}
			return false;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (item.Value == null || !TryGetValue(item.Key, out var value))
			{
				return false;
			}
			if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
			{
				return Remove(item.Key);
			}
			return false;
		}
	}
}