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
		[Fact]
		public void BubbleSort()
		{
			var ar = Vars.Ar;
			var arSorted = Vars.ArSorted;
			for (int i = 0; i < ar.Length; i++)	// 循环的次数
			{
				for (int j = 0; j < ar.Length - 1; j++)	// 循环一次将最大的冒泡到最后边
				{
					if (ar[j] > ar[j + 1])
					{
						var temp = ar[j + 1];
						ar[j + 1] = ar[j];
						ar[j] = temp;
					}
				}
			}
			Assert.Equal(ar, arSorted);
		}
	}
}
