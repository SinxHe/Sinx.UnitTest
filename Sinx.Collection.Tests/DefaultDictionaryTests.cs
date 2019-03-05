using System.Collections.Generic;
using Moq;
using Xunit;

namespace Sinx.Collection.Tests
{
	public class DefaultDictionaryTests
	{
		[Fact]
		public void DefaultDictionary_Add()
		{
			// Arrange
			var compareMock = new Mock<IEqualityComparer<object>>();
			compareMock.Setup(e => e.Equals(1, 1)).Returns(false);
			compareMock.Setup(e => e.GetHashCode(It.IsAny<object>())).Returns<object>(e => (int)e);

			// Act
			var dic = new DefaultDictionary<object, object>(0, compareMock.Object)
			{
				{1, 1}, {1, 2}, {2, 3}
			};

			// Assert
			Assert.Equal(3, dic.Count);
		}

		[Fact]
		public void DefaultDictionary_Init()
		{
			// Arrange & Act
			// Capacity == 0, IEqualityCompare == null(EqualityCompare<TKey>.Default)
			var dic = new DefaultDictionary<string, string>();

			// Assert
			Assert.Empty(dic);
			Assert.Equal(dic.Comparer, EqualityComparer<string>.Default);

			// =========================
			// Arrange & Act
			var dic1 = new DefaultDictionary<string, string>(3);

			// Assert
			Assert.Empty(dic1);
		}
	}
}
