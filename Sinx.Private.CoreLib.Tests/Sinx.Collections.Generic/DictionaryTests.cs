using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sinx.Collections.Generic
{
	public class DictionaryTests
	{
		private readonly Dictionary<int, int> _dic = new();

		[Fact]
		public void Dictionary_Add()
		{
			_dic.Add(1, 1);
			Assert.True(_dic.Single().Key == 1);
			// duplicate
			Assert.ThrowsAny<Exception>(() => _dic.Add(1, 2));
			// 冲突
			var dic = new Dictionary<int, int>(new EqualityComparer()) {{1, 1}, {1, 2}};
		}

		private class EqualityComparer : IEqualityComparer<int>
		{
			public bool Equals(int x, int y)
			{
				return false;
			}

			public int GetHashCode(int obj)
			{
				return obj.GetHashCode();
			}
		}
	}
}