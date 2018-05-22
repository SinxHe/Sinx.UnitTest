using Xunit;

namespace Sinx.Algorithm
{
    public class _03旋转数组
    {
        [Theory]
        [InlineData(new[] { 1, 2, 3, 4 }, 1, new[] { 4, 1, 2, 3 })]
        [InlineData(new[] { 1, 2, 3, 4 }, 2, new[] { 3, 4, 1, 2 })]
        public void Rotate(int[] nums, int k, int[] expect)
        {
            // Arrange & Act
            for (int j = 0; j < k; j++)
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    var temp = nums[0];
                    nums[0] = nums[i];
                    nums[i] = temp;
                }
            }

            // Assert
            Assert.Equal(expect, nums);
        }

        //[Theory]
        //[InlineData(new[] { 1, 2, 3, 4 }, 1, new[] { 4, 1, 2, 3 })]
        //[InlineData(new[] { 1, 2, 3, 4 }, 2, new[] { 3, 4, 1, 2 })]
        //public void Rotate2(int[] nums, int k, int[] expect)
        //{
        //    // Arrange & Act
        //    var beginIndex = nums.Length - k - 1;
        //    for (int i = 0; i < nums.Length; i++)
        //    {
        //        var temp = nums[i];
        //        nums[i] = nums[beginIndex];
        //        nums[beginIndex] = temp;
        //        if (beginIndex == nums.Length)
        //        {
        //            beginIndex = 0;
        //        }
        //    }

        //    // Assert
        //    Assert.Equal(expect, nums);
        //}
    }
}
