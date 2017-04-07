using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Sinx.UnitTest.Algorithm.Sort
{
	public class _04MergeTest
	{
		[Fact]
		public void Merge()
		{
			IEnumerable<int> partition(IEnumerable<int> ie)
			{
				var ints = ie.ToList();
				switch (ints.Count)
				{
					case 1:
						return ints;
					case 2:
						return new[] {Math.Min(ints[0], ints[1]), Math.Max(ints[0], ints[1])};
					default:
						var mid = Convert.ToInt32(ints.Count / 2);
						var left = partition(ints.Take(mid)).ToList();
						var right = partition(ints.Skip(mid).Take(ints.Count - mid)).ToList();
						var sorted = new int[left.Count + right.Count];
						for (int i = 0, il = 0, ir = 0; i < sorted.Length; i++)
						{
							if (left[il] < right[ir])
							{
								sorted[i] = left[il++];
							}
							else
							{
								sorted[i] = right[ir++];
							}
							if (il >= left.Count)
							{
								il--;
								left[il] = int.MaxValue;
							}
							if (ir >= right.Count)
							{
								ir--;
								right[ir] = int.MaxValue;
							}
						}
						return sorted;
				}
			}
			var ar = Vars.Ar;
			var arSorted = Vars.ArSorted;
			ar = partition(ar).ToArray();
			Assert.Equal(ar, arSorted);
		}
	}
}
