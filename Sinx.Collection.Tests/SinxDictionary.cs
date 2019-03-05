using System;
using System.Collections;
using System.Collections.Generic;

namespace Sinx.Collection.Tests
{
	public class SinxDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private Entry[] _entries;
		private int[] _buckets;
		private int _freeCount;

		private readonly IEqualityComparer<TKey> _comparer;

		public int Count { get; }
		public bool IsReadOnly { get; }
		public ICollection<TKey> Keys { get; }
		public ICollection<TValue> Values { get; }

		#region Ctor

		public SinxDictionary(int capacity)
		{
			_entries = new Entry[capacity];
			_buckets = new int[capacity];
			_freeCount = capacity;
			_comparer = EqualityComparer<TKey>.Default;
		}

		#endregion

		#region 增

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			if (item.Key == null)
			{
				throw new ArgumentNullException(nameof(item.Key));
			}

			var hashCode = _comparer.GetHashCode(item.Key);
			// 寻找集合中是否已经存在此数据
			var targetBucket = hashCode % _entries.Length;
			for (var i = _buckets[targetBucket]; i >= 0; i = _entries[i].Next)
			{
				if (_entries[i].HashCode == hashCode && _comparer.Equals(_entries[i].Key, item.Key))
				{
					throw new Exception("duplicate");
				}
			}

		}

		#endregion

		#region 删



		#endregion

		#region 改



		#endregion


		#region 查



		#endregion



		public void Clear()
		{
			throw new System.NotImplementedException();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new System.NotImplementedException();
		}

		public void Add(TKey key, TValue value)
		{
			throw new System.NotImplementedException();
		}

		public bool ContainsKey(TKey key)
		{
			throw new System.NotImplementedException();
		}

		public bool Remove(TKey key)
		{
			throw new System.NotImplementedException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			throw new System.NotImplementedException();
		}

		public TValue this[TKey key]
		{
			get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException();
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#region Nested Type

		private struct Entry
		{
			public int HashCode { get; set; }
			public int Next { get; set; }
			public TKey Key { get; set; }
			public TValue Value { get; set; }
		}

		#endregion
	}
}
