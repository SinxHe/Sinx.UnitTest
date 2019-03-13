using System;
using System.Collections;
using System.Collections.Generic;

namespace Sinx.Collection.Tests
{
	public class SinxDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private int _count;
		private int _freeList;
		private int _freeListCount;
		private readonly IEqualityComparer<TKey> _comparer;
		private Entry[] _entries;
		// 如果没有buckets, 就无法顺序的将item添加到_entries
		// 如果不能顺序的将item添加到_entries, 插入点索引的寻找
		// 需要扫描_entries寻找空节点, 这会导致时间复杂度变成O(n)
		private int[] _buckets; // /'bʌkɪt/ 
		public int Count => _count - _freeListCount;
		public bool IsReadOnly { get; }
		public ICollection<TKey> Keys { get; }
		public ICollection<TValue> Values { get; }
		public TValue this[TKey key]
		{
			get
			{
				var i = FindEntry(key);
				if (i < 0)
				{
					throw new KeyNotFoundException();
				}

				return _entries[i].Value;
			}
			set => Insert(key, value, false);
		}

		#region Ctor

		public SinxDictionary(int capacity, IEqualityComparer<TKey> comparer = null)
		{
			_comparer = comparer ?? EqualityComparer<TKey>.Default;
			_entries = new Entry[capacity]; //  /kə'pæsəti/ 
			_buckets = new int[capacity];
			for (var i = 0; i < capacity; i++)
			{
				_entries[i].Next = -1;
				_buckets[i] = -1;
			}
		}

		#endregion

		#region 增

		public void Add(TKey key, TValue value)
		{
			Insert(key, value, true);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		#endregion

		#region 删

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			var i = FindEntry(item.Key);
			if (i >= 0 && EqualityComparer<TValue>.Default.Equals(item.Value, _entries[i].Value))
			{
				return Remove(item.Key);
			}
			return false;
		}

		public bool Remove(TKey key)
		{
			var hashCode = _comparer.GetHashCode(key);
			var bucketIndex = hashCode % _buckets.Length;
			var last = -1;
			for (int i = _buckets[bucketIndex]; i >= 0; last = i, i = _entries[i].Next)
			{
				if (hashCode == _entries[i].HashCode &&
					_comparer.Equals(key, _entries[i].Key))
				{
					if (last < 0)
					{
						_buckets[bucketIndex] = _entries[i].Next;
					}
					else
					{
						_entries[last].Next = _entries[i].Next;
					}
					_entries[i].Next = _freeList;
					_freeList = i;
					_freeListCount++;
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			_buckets = new int[3];
			_entries = new Entry[3];
			_count = 0;
			_freeList = -1;
			_freeListCount = 0;
		}

		#endregion

		#region 改

		#endregion

		#region 查

		public bool ContainsKey(TKey key)
		{
			return FindEntry(key) >= 0;
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			var index = FindEntry(item.Key);
			if (index < 0)
			{
				return false;
			}
			var value = _entries[index].Value;
			return EqualityComparer<TValue>.Default.Equals(item.Value, value);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			var index = FindEntry(key);
			if (index < 0)
			{
				value = default;
				return false;
			}
			value = _entries[index].Value;
			return true;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			var enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				array[arrayIndex++] = enumerator.Current;
			}
		}

		#endregion

		#region Private

		private void Insert(TKey key, TValue value, bool isAdd)
		{
			if (_count == _entries.Length)
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

			int index;
			if (_freeListCount > 0)
			{
				index = _freeList;
				_freeList = _entries[_freeList].Next;
				_freeListCount--;
			}
			else
			{
				if (_count == _entries.Length)
				{
					throw new Exception("over flow");
				}
				index = _count;
				_count++;
			}

			_entries[index].Key = key;
			_entries[index].Value = value;
			_entries[index].HashCode = hashCode;
			_entries[index].Next = _buckets[bucketIndex];
			_buckets[bucketIndex] = index;
		}

		private int FindEntry(TKey key)
		{
			if (key?.Equals(default) ?? true)
			{
				throw new ArgumentException(nameof(key));
			}

			var hashCode = _comparer.GetHashCode(key);
			var bucketIndex = hashCode % _buckets.Length;
			for (var i = _buckets[bucketIndex]; i >= 0; i = _entries[i].Next)
			{
				if (_entries[i].HashCode == hashCode)
				{
					return i;
				}
			}

			return -1;
		}

		#endregion

		#region Nested Type

		private struct Entry
		{
			public int HashCode { get; set; }   // 当为空时设置为0
			public int Next { get; set; }
			public TKey Key { get; set; }
			public TValue Value { get; set; }
		}

		private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			private readonly SinxDictionary<TKey, TValue> _dictionary;
			private int _index = -1;
			public KeyValuePair<TKey, TValue> Current { get; private set; }

			object IEnumerator.Current => Current;

			public Enumerator(SinxDictionary<TKey, TValue> dictionary)
			{
				_dictionary = dictionary;
			}

			public bool MoveNext()
			{
				while (_index++ < _dictionary.Count)
				{
					var entry = _dictionary._entries[_index];
					if (entry.HashCode > 0)
					{
						Current = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
						return true;
					}
				}
				return false;
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
