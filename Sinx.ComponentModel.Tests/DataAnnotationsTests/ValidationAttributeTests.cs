using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;


namespace Sinx.ComponentModel.Tests.DataAnnotationsTests
{
	public class ValidationAttributeTests
	{
		[Fact]
		public void ValidationAttribute_Usage()
		{
			// Arrange
			var poco = new MyPOCO();
			var propInfo = typeof(MyPOCO)
				.GetProperty("MyProperty");
			var validation = propInfo
				.GetCustomAttributes(true)
				.Single(e => e is MyValidationAttribute) as MyValidationAttribute;
			var propValue = propInfo.GetValue(poco);

			// Act
			var isValid = validation.IsValid(propValue);

			// Assert
			Assert.True(isValid);
		}




	}
}
