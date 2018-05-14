namespace Sinx.Refactor.Tests
{
    public class ChildrensPrice03 : Price03
    {
	    public override int GetPriceCode()
	    {
			return Movie03.CHILDRENS;
	    }

		public override double GetCharge(int daysRenged)
		{
			var result = 1.5;
			if (daysRenged > 3)
			{
				result += (daysRenged - 3) * 1.5;
			}
			return result;
		}
	}
}
