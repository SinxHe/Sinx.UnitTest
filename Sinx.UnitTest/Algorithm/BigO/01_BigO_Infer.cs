using Xunit;

// ReSharper disable InconsistentNaming

namespace Sinx.UnitTest.Algorithm.BigO
{
	public class _01_BigO_Compute
	{
		/// <summary>
		/// 推断大O时间复杂度
		/// </summary>
		[Fact]
		public void BigO_Infer()
		{
			int n = 10;
			// 常数阶 - O(1)
			var r = n * n;	// 计算次数与n无关
			// 线性阶 - O(n)
			for (int i = 0; i < n; i++)
			{
				// 每次前进常数步
			}
			// 对数阶 - O(lgn)
			for (int i = 0; i < n; i *= 2)
			{
				// 每次前进步数线性增长
			}
			// 平方阶 - O(n方)
			for (int i = 0; i < n; i++)
			{
				// 每次前进常数步
				for (int j = 0; j < n; j++)
				{
					// 每次前进常数步
				}
			}
		}
	}
}
