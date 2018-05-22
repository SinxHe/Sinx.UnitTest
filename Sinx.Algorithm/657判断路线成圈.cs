using Xunit;

namespace Sinx.Algorithm
{
    public class _657判断路线成圈
    {
        [Theory]
        [InlineData("U", false)]
        [InlineData("ULDR", true)]
        public void JudgeCircle(string moves, bool expect)
        {
            // Arrange
            var horizontal = 0;
            var vertical = 0;
            foreach (var @char in moves)
            {
                switch (@char)
                {
                    case 'U':
                        vertical += 1;
                        break;
                    case 'D':
                        vertical += -1;
                        break;
                    case 'L':
                        horizontal += -1;
                        break;
                    case 'R':
                        horizontal += 1;
                        break;
                }
            }

            Assert.Equal(expect, horizontal + vertical == 0);
        }
    }
}
