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
				.GetCustomAttributes(typeof(MyValidationAttribute), true)
				.Single() as MyValidationAttribute;
			var propValue = propInfo.GetValue(poco);

			// Act
			var isValid = validation.IsValid(propValue);

			// Assert
			Assert.True(isValid);
		}

		public class MyValidationAttribute : ValidationAttribute
		{
			public override bool IsValid(object value)
			{
				return value.Equals(1);
			}
		}

		public class MyPOCO
		{
			[MyValidation] public int MyProperty => 1;
		}
	}
}
