using System;
using System.Collections.Generic;

namespace Sinx.Collection.Tests
{
	public class AlgorithmHashTable<TKey, TValue>
	{
		private readonly Entry[] _entries;

		public AlgorithmHashTable(int capacity = 3)
		{
			_entries = new Entry[capacity];
		}

		public void Add(TKey key, TValue value)
		{
			LinearSearchAdd(key, value);
		}

		public KeyValuePair<TKey, TValue> this[TKey key] => LinearSearch(key);

		// 线性查找添加
		private void LinearSearchAdd(TKey key, TValue value)
		{
			var probeBeginIndex = GetProbeBeginIndex(key);
			for (int i = probeBeginIndex; i > 0; i = LinearSearchMoveNext(probeBeginIndex, i))
			{
				if (_entries[i].Equals(default) || _entries[i].IsDelete)
				{
					_entries[i] = new Entry
					{
						Key = key,
						Value = value
					};
				}
			}

			throw new Exception("dictionary overflow");
		}

		// 线性查找
		private KeyValuePair<TKey, TValue> LinearSearch(TKey key)
		{
			var probeBeginIndex = GetProbeBeginIndex(key);
			for (var i = probeBeginIndex; i > 0; i = LinearSearchMoveNext(probeBeginIndex, i))
			{
				if (!_entries[i].Equals(default) && !_entries[i].IsDelete)
				{
					return new KeyValuePair<TKey, TValue>(_entries[i].Key, _entries[i].Value);
				}
			}

			return default;
		}

		private int GetProbeBeginIndex(TKey key)
		{
			var hashCode = key.GetHashCode();
			var probeBeginIndex = hashCode % _entries.Length;
			return probeBeginIndex;
		}

		private int LinearSearchMoveNext(int begin, int i)
		{
			if (++i == _entries.Length)
			{
				i = 0;
			}
			if (i == begin - 1)
			{
				i = -1;
			}
			return i;
		}

		private struct Entry
		{
			public TKey Key { get; set; }
			public TValue Value { get; set; }
			public bool IsDelete { get; set; }
		}
	}
}
