using System.ComponentModel.DataAnnotations;

namespace Sinx.ComponentModel.Tests
{
	public class MyValidationAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			return value.Equals(1);
		}
	}
}
