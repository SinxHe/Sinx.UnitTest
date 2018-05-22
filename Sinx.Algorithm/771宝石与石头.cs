using Xunit;

namespace Sinx.Algorithm
{
    public class _771宝石与石头
    {
        [Theory]
        [InlineData("aA", "aAAbbbb", 3)]
        [InlineData("z", "ZZ", 0)]
        public void NumJewelsInStones(string J, string S, int expect)
        {
            // Arrange & Act
            int count = 0;
            foreach (var sElement in S)
            {
                foreach (var jElement in J)
                {
                    if (jElement == sElement)
                    {
                        count++;
                    }
                }
            }

            // Assert
            Assert.Equal(expect, count);
        }
    }
}
