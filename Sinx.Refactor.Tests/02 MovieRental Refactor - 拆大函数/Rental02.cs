namespace Sinx.Refactor.Tests
{
    internal class Rental02
    {
		public Movie02 Movie { get; }
		public int DaysRented { get; }
		public Rental02(Movie02 movie, int daysRented)
		{
			Movie = movie;
			DaysRented = daysRented;
		}

	    public double GetCharge()
	    {
		    double result = 0;
		    switch (Movie.PriceCode)
		    {
			    case Movie02.REGULAR:
				    result += 2;
				    if (DaysRented > 2)
				    {
					    result += (DaysRented - 2) * 1.5;
				    }
				    break;
			    case Movie02.NEW_RELEASE:
				    result += DaysRented * 3;
				    break;
			    case Movie02.CHILDRENS:
				    result += 1.5;
				    if (DaysRented > 3)
				    {
					    result += (DaysRented - 3) * 1.5;
				    }
				    break;
		    }
		    return result;
	    }

	    public int GetFrequentRenterPoints()
	    {
		    var frequentRenterPoints = 0;
		    frequentRenterPoints++;
			if (Movie.PriceCode == Movie02.NEW_RELEASE && DaysRented > 1)
		    {
			    frequentRenterPoints++;
		    }
		    return frequentRenterPoints;
	    }
    }
}
