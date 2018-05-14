namespace Sinx.Refactor.Tests
{
    public class RegularPrice03 : Price03
    {
	    public override int GetPriceCode()
	    {
			return Movie03.REGULAR;
	    }

	    public override double GetCharge(int daysRenged)
	    {
		    var result = 2.0;
		    if (daysRenged > 2)
		    {
			    result += (daysRenged - 2) * 1.5;
		    }
			return result;
	    }
    }
}
