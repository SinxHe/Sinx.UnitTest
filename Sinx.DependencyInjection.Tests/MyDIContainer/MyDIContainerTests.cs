using Xunit;

namespace Sinx.DependencyInjection.Tests.MyDIContainer
{
	public class MyDIContainerTests
	{
		[Fact]
		public void MyDIContainer_AddService()
		{
			// Arrange
			var container = new MyDIContainer();
			var element0 = new DIContainerElement(typeof(int), 1);
			int i = 0;
			var element1 = new DIContainerElement(typeof(long), 
				DIContainerElement.IDIContainerElementLifecyle.Transient,
				() => i++);

			// Act
			container.AddService(element0);
			container.AddService(element1);

			// Assert
			Assert.NotEqual(0, container.GetService(typeof(int)));
			Assert.Equal(0, container.GetService(typeof(long)));
			Assert.Equal(1, container.GetService(typeof(long)));
		}
	}
}
