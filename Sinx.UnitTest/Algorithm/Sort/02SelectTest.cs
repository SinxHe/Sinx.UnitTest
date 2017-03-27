using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.Algorithm.Sort
{
	// ReSharper disable once InconsistentNaming
	public class _02SelectTest
	{
		[Fact]
		public void SelectSort()
		{
			var ar = Vars.Ar;
			var arSorted = Vars.ArSorted;
			for (int i = 0; i < ar.Length; i++)	// 循环次数, 对应每一个排好序的位置
			{
				int small = i;
				for (int j = i; j < arSorted.Length; j++)
				{
					if (ar[small] > ar[j])
					{
						small = j;
					}
				}
				var temp = ar[small];
				ar[small] = ar[i];
				ar[i] = temp;
			}
			Assert.Equal(ar, arSorted);
		}
	}
}
