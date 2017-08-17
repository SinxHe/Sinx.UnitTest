using System.Linq;
using Xunit;

namespace Sinx.UnitTest.System.Linq
{
	public class EnumerableTests
	{
		#region 投影操作 

		#region Select / select x into xx

		[Fact]
		public void SelectTest_Array_ArrayValueAndIndex()
		{
			// Select(TElement, int), int 为当前元素的索引
			var r0 = Enumerable.Range(0, 100).Select((e, i) => new { e, i });
			r0.ToList().ForEach(m => Assert.Equal(m.e, m.i));
		}

		[Fact]
		public void SelectTest_SelectInto()
		{
			// 查询语法
			var r0 =
				from n in Enumerable.Range(0, 100)  // 一直隐式在各个关键字之间传递 n
				select n + 1 into num               // 将隐式传递的 n 转换成 num
				where num < 10
				select num;                         // 必须使用select结尾

			// 方法语法
			var r1 = Enumerable.Range(0, 100)   // 这里也是在传递 n
				.Select(n => n + 1)
				.Where(n => n < 10);            // 没有使用Select结尾

			Assert.Equal(r0, r1);
		}
		#endregion

		#region SelectMany

		[Fact]
		public void SelectMany()
		{
			// IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
			// Func<TSource, IEnumerable<TResult> => 传入一个元素, 返回一个集合 => SelectMany 将返回的集合拼接在一起

			var r0 = new[] { "123", "45", "67", "8" }.SelectMany(m => m);   // 直接将字符串m作为集合返回, 让SelectMany进行拼接
			Assert.Equal(r0, new[] { '1', '2', '3', '4', '5', '6', '7', '8' });

			var r1 = Enumerable.Range(0, 4).SelectMany(n => Enumerable.Range(0, n));
			Assert.Equal(r1, new[] { 0, 0, 1, 0, 1, 2 });

			// 将源序列中的每个元素跟选择序列中的每个元素一起进行操作
			var r2 = Enumerable.Range(0, 4).SelectMany(n => Enumerable.Range(0, n), (nS, nT) => nS + " - " + nT);
			Assert.Equal(r2, new[] { "1 - 0", "2 - 0", "2 - 1", "3 - 0", "3 - 1", "3 - 2" });
		}

		#endregion

		#endregion
	}
}
