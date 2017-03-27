using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.Algorithm.Sort
{
	// ReSharper disable once InconsistentNaming
	public class _01BubbleTest
	{
		private readonly int[] _ar = {3, 44, 38, 5, 47, 15, 36, 26, 27, 2, 46, 4, 19, 50, 48};
		private readonly int[] _arSorted = {2, 3, 4, 5, 15, 19, 26, 27, 36, 38, 44, 46, 47, 48, 50};
		[Fact]
		public void BubbleSort()
		{
			for (int i = 0; i < _ar.Length; i++)	// 循环的次数
			{
				for (int j = 0; j < _ar.Length - 1; j++)	// 循环一次将最大的冒泡到最后边
				{
					if (_ar[j] > _ar[j + 1])
					{
						var temp = _ar[j + 1];
						_ar[j + 1] = _ar[j];
						_ar[j] = temp;
					}
				}
			}
			Assert.Equal(_ar, _arSorted);
		}
	}
}
