using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace Sinx.UnitTest.Algorithm
{
	public class LeetCodeTest
	{
		[Fact]
		public void TwoSumTest()
		{
			var ns = new[] { 2, 7, 11, 15 };
			var result = TwoSum(ns, 9);
			int[] TwoSum(int[] nums, int target)
			{
				int len = nums.Length;
				Dictionary<int, int> dic = new Dictionary<int, int>();
				for (int i = 0; i < len; i++)
				{
					int tmp = nums[i];
					if (dic.Keys.Contains(target - tmp))
						return new int[] { dic[target - tmp], i };
					dic[tmp] = i;
				}
				return new int[] { };


				//var n = nums.Length - 1;
				//for (int i = 0; i < n; i++)
				//{
				//	for (int j = i + 1; j < nums.Length; j++)
				//	{
				//		if (nums[i] + nums[j] == target)
				//		{
				//			return new[] { i, j };
				//		}
				//	}
				//}
				//return new int[0];
			}
		}
	}
}
