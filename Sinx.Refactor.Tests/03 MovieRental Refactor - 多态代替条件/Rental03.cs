namespace Sinx.Refactor.Tests
{
    internal class Rental03
    {
		public Movie03 Movie { get; }
		public int DaysRented { get; }
		public Rental03(Movie03 movie, int daysRented)
		{
			Movie = movie;
			DaysRented = daysRented;
		}

	    public double GetCharge()
	    {
		    return Movie.GetCharge(DaysRented);
	    }

	    public int GetFrequentRenterPoints()
	    {
		    return Movie.GetFrequentRenterPoints(DaysRented);
	    }
    }
}
