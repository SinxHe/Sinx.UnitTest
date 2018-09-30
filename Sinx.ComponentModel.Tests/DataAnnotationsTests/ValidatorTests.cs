using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Sinx.ComponentModel.Tests.DataAnnotationsTests
{
	public class ValidatorTests
	{
		[Fact]
		public void Validator_Validate()
		{
			// Arrange
			var poco = new MyPOCO();
			poco.MyProperty = 2;
			var ctx = new ValidationContext(poco);

			// Act
			var isObjectValidate = Validator.TryValidateObject(poco, ctx, null);
			var results = new List<ValidationResult>();
			ctx.MemberName = "MyProperty";
			var isPropertyValidate = Validator.TryValidateProperty(poco.MyProperty, ctx, results);
			var isAllPropertyValidate = Validator.TryValidateObject(poco, ctx, results, true);

			// Assert
			Assert.True(isObjectValidate);
		}
	}
}
