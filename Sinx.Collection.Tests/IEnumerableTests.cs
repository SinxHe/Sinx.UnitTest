using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sinx.Collection.Tests
{
	public class IEnumerableTests
	{
		/// <summary>
		/// 其实IEnumerable就是Enumerator的工厂,
		/// 为了屏蔽Enumerator的迭代状态用的
		/// 即: 每次使用Enumerator都是一个初始状态的
		/// 假设我从你手里获取一个序列, 如果你给我一个enumerator,
		/// 我则不知道你之前有没有迭代过这个序列化器, 且我只能使用他一次(如果reset方法不好用)
		/// 如果是给我一个工厂的话就好所说了
		/// </summary>
		[Fact]
		public void IEnumerable_GetEnumerator()
		{
			// Arrange
			var sequence = new MyEnumerable();

			// Act
			var enumerator = sequence.GetEnumerator();

			// Assert
			var list = new List<char>();
			while (enumerator.MoveNext())
			{
				list.Add((char)enumerator.Current);
			}
			Assert.Equal("hello world", list.Aggregate(string.Empty, (c, i) => c + i));
		}

		private class MyEnumerable : IEnumerable
		{
			public IEnumerator GetEnumerator()
			{
				return new MyEnumerator();
			}
		}

		private class MyEnumerator : IEnumerator
		{
			private int _index;
			private const string _word = "hello world";

			public object Current => _word[_index];

			public bool MoveNext()
			{
				return _index < _word.Length;
			}

			public void Reset()
			{
				_index = 0;
			}
		}
	}
}
