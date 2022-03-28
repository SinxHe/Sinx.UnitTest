using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sinx.Collections.Generic
{
	public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		// 数组的大小由Capacity控制, 所以这里先不赋值
		private int[]? _buckets;
		private Entry[]? _entries;
		private int _count;
		private int _freeList;
		private int _freeCount;
		private const int StartOfFreeList = -3;
		private IEqualityComparer<TKey>? _comparer;
		private int _version;

		public int Count => _count - _freeCount;
		public bool IsReadonly => false;
		public IEnumerable<TKey> Keys => new Enumerable(this).Select(e => e.Key);
		public IEnumerable<TValue> Values => new Enumerable(this).Select(e => e.Value);

		public Dictionary() : this(0) { }

		public Dictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

		public Dictionary(int capacity, IEqualityComparer<TKey>? comparer = null)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}
			if (capacity > 0)
			{
				Initialize(capacity);
			}
			// 先判断comparer是否为null, 避免默认comparer初始化
			if (comparer is not null && comparer != EqualityComparer<TKey>.Default)
			{
				_comparer = comparer;
			}
			// todo non-randomized comparer improve pref
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new Enumerator(this);
		}

		public void Add(TKey key, TValue value)
		{
			TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
		}

		public bool Remove(TKey key)
		{
			throw new System.NotImplementedException();
		}

		public TValue this[TKey key]
		{
			get => throw new System.NotImplementedException();
			set => throw new System.NotImplementedException();
		}

		public bool ContainsKey(TKey key)
		{
			throw new System.NotImplementedException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			throw new System.NotImplementedException();
		}

		public void Clear()
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		private void Initialize(int capacity)
		{
			var size = HashHelpers.GetPrime(capacity);
			_buckets = new int[size];
			_entries = new Entry[size];
			_freeList = -1;
		}

		private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (_buckets == null)
			{
				Initialize(0);
			}
			// hashCode => bucket => index
			var hashCode = GetHashCode(key);
			uint collisionCount = 0;
			ref var bucket = ref GetBucket(hashCode);
			// value in _buckets is 1-based
			// 如果是一个新bucket, 则为0, i为-1, (uint)-1 >= (uint)_entries.Length  
			var i = bucket - 1;
			while (true)
			{
				if ((uint) i >= (uint) _entries!.Length)
				{
					break;
				}
				if (_entries[i].HashCode == hashCode && Equals(_entries[i].Key, key))
				{
					switch (behavior)
					{
						case InsertionBehavior.None:
							return false;
						case InsertionBehavior.OverwriteExisting:
							_entries[i].Value = value;
							return true;
						case InsertionBehavior.ThrowOnExisting:
							throw new DuplicateWaitObjectException();
						default:
							throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null);
					}
				}
				// 如果冲突, 则寻找下一个, 如果没有下一个, i为`-1`
				i = _entries[i].Next;
				if (++collisionCount > (uint) _entries.Length)
				{
					// The chain of entries forms a loop; which means a concurrent update has happened.
					// Break out of the loop and throw, rather than looping forever.
					throw new NotSupportedException("concurrent");
				}
			}
			// 如果有删除数据后留下来的空位, 则优先使用这些空位
			int index;
			if (_freeCount > 0)
			{
				index = _freeList;
				_freeList = StartOfFreeList - _entries[_freeList].Next;
				_freeCount--;
			}
			else
			{
				if (_count == _entries.Length)
				{
					Resize();
					bucket = ref GetBucket(hashCode);
				}
				index = _count;
				_count++;
			}
			ref var entry = ref _entries![index];
			entry.HashCode = hashCode;
			entry.Next = bucket - 1;
			entry.Key = key;
			entry.Value = value;
			// index = bucket - 1, 所以这里计算bucket的时候要`+1`
			bucket = index + 1;
			_version++;
			// Value types never rehash
			if (!typeof(TKey).IsValueType &&
			    collisionCount >
			    HashHelpers.HashCollisionThreshold /* && _comparer is NonRandomizedStringEqualityComparer*/)
			{
				// If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
				// i.e. EqualityComparer<string>.Default.
				Resize(_entries.Length, true);
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref int GetBucket(uint hashCode)
		{
			return ref _buckets![hashCode % (uint) _buckets.Length];
		}

		private void Resize() => Resize(HashHelpers.ExpandPrime(_count), false);

		private void Resize(int newSize, bool forceNewHashCodes)
		{
			var entries = new Entry[newSize];
			Array.Copy(_entries!, entries, _count);
			if (!typeof(TKey).IsValueType && forceNewHashCodes)
			{
				// todo randomized equality comparer
				_comparer = EqualityComparer<TKey>.Default;
				for (var i = 0; i < _count; i++)
				{
					if (entries[i].Next >= -1)
					{
						entries[i].HashCode = (uint) _comparer.GetHashCode(entries[i].Key);
					}
				}
				if (ReferenceEquals(_comparer, EqualityComparer<TKey>.Default))
				{
					_comparer = null;
				}
			}
			_buckets = new int[newSize];
			for (var i = 0; i < _count; i++)
			{
				if (_entries![i].Next >= -1)
				{
					ref var bucket = ref GetBucket(entries[i].HashCode);
					entries[i].Next = bucket - 1;
					bucket = i + 1;
				}
			}
			_entries = entries;
		}

		private uint GetHashCode(TKey key)
		{
			var hashCode = _comparer?.GetHashCode(key) ?? key!.GetHashCode();
			return (uint) hashCode;
		}

		private bool Equals(TKey x, TKey y)
		{
			return _comparer?.Equals(x, y) ?? EqualityComparer<TKey>.Default.Equals(x, y);
		}

		private struct Entry
		{
			public uint HashCode { get; set; }

			/// <summary>
			/// 0-based index of next entry in chain: -1 means end of chain
			/// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
			/// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
			/// </summary>
			public int Next { get; set; }

			public TKey Key { get; set; }
			public TValue Value { get; set; }
		}
		
		private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			private readonly Dictionary<TKey, TValue> _dic;
			private readonly int _version;
			private int _index;

			public KeyValuePair<TKey, TValue> Current { get; private set; }

			public Enumerator(Dictionary<TKey, TValue> dic)
			{
				_dic = dic;
				_version = _dic._version;
				_index = 0;
				Current = default;
			}
			
			public bool MoveNext()
			{
				if (_version != _dic._version)
				{
					throw new NotSupportedException("Concurrent");
				}
				while (_index < _dic._count)
				{
					ref var entry = ref _dic._entries![_index++];
					if (entry.Next >= -1)
					{
						Current = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
						return true;						
					}
				}
				_index = _dic._count + 1;
				Current = default;
				return false;
			}

			public void Reset()
			{
				_index = 0;
				Current = default;
			}

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}
		}
		
		private readonly struct Enumerable : IEnumerable<KeyValuePair<TKey, TValue>>
		{
			private readonly Dictionary<TKey, TValue> _dic;

			public Enumerable(Dictionary<TKey, TValue> dic)
			{
				_dic = dic;
			}
			
			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return new Enumerator(_dic);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}