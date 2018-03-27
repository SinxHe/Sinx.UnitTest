using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sinx.Algorithm
{
    public class _01ArrayTests
    {
        [InlineData(new[] { 1, 1, 2 })]
        [InlineData(new[] { 1, 1, 2, 2 })]
        [Theory]
        public void Array_Reverse_Algorithm(int[] nums)
        {
            var expect = nums.Reverse().ToArray();
            // 实验原地算法
            var mid = (nums.Length - 1) / 2.0;
            mid = mid % 1 > 0 ? (int)mid + 1 : mid;
            for (int i = 0; i < mid; i++)
            {
                var temp = nums[i];
                nums[i] = nums[nums.Length - i - 1];
                nums[nums.Length - 1 - i] = temp;
            }

            Assert.Equal(nums, expect);
        }

        /// <summary>
        /// 原地算法: https://baike.baidu.com/item/%E5%8E%9F%E5%9C%B0%E7%AE%97%E6%B3%95
        /// 给定一个有序数组，你需要原地删除其中的重复内容，使每个元素只出现一次,并返回新的长度。
        /// 不要另外定义一个数组，您必须通过用 O(1) 额外内存原地修改输入的数组来做到这一点。
        /// 给定数组: nums = [1,1,2],
        /// 你的函数应该返回新长度 2, 并且原数组nums的前两个元素必须是1和2
        /// 不需要理会新的数组长度后面的元素
        /// </summary>
        [InlineData(new[] { 1, 1, 2 }, new[] { 1, 2 })]
        [InlineData(new[] { 1, 1, 2, 2 }, new[] { 1, 2 })]
        [InlineData(new[] { 1, 2, 3, 3, 4, 2, 1 }, new[] { 1, 2, 3, 4 })]
        [InlineData(new[] { 1, 1, 2, 3 }, new[] { 1, 2, 3 })]
        [Theory]
        public void Array_Distinct_Algorithm(int[] nums, int[] expect)
        {
            // Act
            var exchangeNum = 0;
            // 箭头
            for (int i = 0; i < nums.Length; i++)
            {
                // 判断是否找完
                if (i + exchangeNum >= nums.Length)
                {
                    break;
                }
                // 箭头左边 箭头L
                for (int k = 0; k < i; k++)
                {
                    // 判断前面是不是有重复的
                    // 有
                    if (nums[k] == nums[i])
                    {
                        // 将这个元素放到从结尾往回滑动的元素位置
                        for (int j = i; j < nums.Length - 1; j++)
                        {
                            var temp = nums[j];
                            nums[j] = nums[j + 1];
                            nums[j + 1] = temp;
                        }
                        exchangeNum++;
                        // 对交换的元素再次进行验证
                        i--;
                    }
                    // 没有
                    else
                    {
                        // 箭头L向后移动
                    }
                }
            }

            var lengthOfNewArray = nums.Length - exchangeNum;
            // Assert
            Assert.Equal(nums.Take(lengthOfNewArray), expect);
        }

        [InlineData(new[] { 1, 1, 2 }, new[] { 1, 2 })]
        [InlineData(new[] { 1, 1, 2, 2 }, new[] { 1, 2 })]
        [InlineData(new[] { 1, 2, 3, 3, 4, 2, 1 }, new[] { 1, 2, 3, 4 })]
        [InlineData(new[] { 1, 1, 2, 3 }, new[] { 1, 2, 3 })]
        [Theory]
        public void Array_Distinct_Algorithm_Best(int[] nums, int[] expect)
        {
            var len = nums.Length;
            if (len != 0)
            {
                var count = 1;
                for (var i = 1; i < len; i++)
                {
                    if (nums[i] == nums[i - 1]) continue;
                    nums[count] = nums[i];
                    count++;
                }
            }
            Assert.Equal(nums.Take(expect.Length), expect);
        }
    }
}
