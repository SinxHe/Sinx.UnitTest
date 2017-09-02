using System.Linq;
using System.Collections.Generic;

namespace Sinx.UnitTest.DesignPattern
{
	public class StrategyPattern
	{
		public void StrategyPatternTest()
		{
			var enums = Enumerable.Range(1, 10);
			var mergeContext = new SortContext(new ConcreteStrategyMerge());
			var quickContext = new SortContext(new ConcreteStrategyQuick());
			var enums1 = mergeContext.Sort(enums);
			var enums2 = quickContext.Sort(enums1);
		}

		internal interface ISortStrategy
		{
			IEnumerable<int> Sort(IEnumerable<int> enums);
		}

		internal class ConcreteStrategyMerge : ISortStrategy
		{
			public IEnumerable<int> Sort(IEnumerable<int> enums)
			{
				return enums;
			}
		}

		internal class ConcreteStrategyQuick : ISortStrategy
		{
			public IEnumerable<int> Sort(IEnumerable<int> enums)
			{
				return enums;
			}
		}

		internal class SortContext
		{
			private ISortStrategy _sortStrategy;

			internal SortContext(ISortStrategy sortStrategy)
			{
				_sortStrategy = sortStrategy;
			}

			internal IEnumerable<int> Sort(IEnumerable<int> enums)
			{
				return _sortStrategy.Sort(enums);
			}
		}
	}
}
