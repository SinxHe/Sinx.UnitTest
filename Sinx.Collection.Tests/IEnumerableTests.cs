using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sinx.Collection.Tests
{
	public class IEnumerableTests
	{
		/// <summary>
		/// ��ʵIEnumerable����Enumerator�Ĺ���,
		/// Ϊ������Enumerator�ĵ���״̬�õ�
		/// ��: ÿ��ʹ��Enumerator����һ����ʼ״̬��
		/// �����Ҵ��������ȡһ������, ��������һ��enumerator,
		/// ����֪����֮ǰ��û�е�����������л���, ����ֻ��ʹ����һ��(���reset����������)
		/// ����Ǹ���һ�������Ļ��ͺ���˵��
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
