namespace Sinx.ComponentModel.Tests.DataAnnotationsTests
{
	public class MyPOCO
	{
		public MyPOCO()
		{
			MyProperty = 1;
		}
		[MyValidation]
		public int MyProperty { get; set; }
	};
}