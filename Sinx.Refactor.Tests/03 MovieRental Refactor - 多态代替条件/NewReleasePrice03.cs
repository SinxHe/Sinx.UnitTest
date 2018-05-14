namespace Sinx.Refactor.Tests
{
    public class NewReleasePrice03 : Price03
    {
	    public override int GetPriceCode()
	    {
		    return Movie03.NEW_RELEASE;
	    }

		public override double GetCharge(int daysRenged)
		{
			return daysRenged * 3;
		}

		public override int GetFrequentRenterPoints(int daysRented)
		{
			return daysRented > 1 ? 2 : 1;
		}
	}
}
