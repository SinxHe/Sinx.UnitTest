using System;
using System.Collections;
using System.Collections.Generic;

namespace Sinx.Collection.Tests
{
	public class SinxDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly IEqualityComparer<TKey> _comparer;
		private readonly Entry[] _entries;
		private int _freeList;
		private int _freeListCount;
		// 如果没有buckets, 就无法顺序的将item添加到_entries
		// 如果不能顺序的将item添加到_entries, 则freeList的寻找
		// 需要扫描_entries寻找空节点, 这会导致时间复杂度变成lg(n)
		private readonly int[] _buckets; // /'bʌkɪt/ 
		public int Count { get; }
		public bool IsReadOnly { get; }
		public ICollection<TKey> Keys { get; }
		public ICollection<TValue> Values { get; }
		public TValue this[TKey key]
		{
			get => throw new System.NotImplementedException();
			set => Insert(key, value, false);
		}

		#region Ctor

		public SinxDictionary(int capacity, IEqualityComparer<TKey> comparer = null)
		{
			_comparer = comparer ?? EqualityComparer<TKey>.Default;
			_entries = new Entry[capacity]; //  /kə'pæsəti/ 
			_buckets = new int[capacity];
			_freeListCount = capacity;
			for (var i = 0; i < capacity; i++)
			{
				_entries[i].Next = -1;
				_buckets[i] = -1;
			}
		}

		#endregion

		#region 增

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Insert(item.Key, item.Value, true);
		}

		#endregion

		#region 删



		#endregion

		#region 改



		#endregion


		#region 查



		#endregion

		#region Private

		private void Insert(TKey key, TValue value, bool isAdd)
		{
			if (_freeListCount <= 0)
			{
				throw new Exception("over flow");
			}
			var hashCode = _comparer.GetHashCode(key);
			var bucketIndex = hashCode % _buckets.Length;
			// 进行重复检查, 如果 isAdd != true, 顺便更改值
			for (var i = _buckets[bucketIndex]; i >= 0; i = _entries[i].Next)
			{
				if (hashCode == _entries[i].HashCode && _comparer.Equals(key, _entries[i].Key))
				{
					if (isAdd)
					{
						throw new Exception("duplicate");
					}
					_entries[i].Value = value;
				}
			}

			_entries[_freeList].Key = key;
			_entries[_freeList].Value = value;
			_entries[_freeList].HashCode = hashCode;
			_entries[_freeList].Next = _buckets[bucketIndex];
			_buckets[bucketIndex] = _freeList;
			_freeList++;
			_freeListCount--;
		}

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
