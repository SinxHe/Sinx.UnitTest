namespace Sinx.Refactor.Tests._01_MovieRental_Simple
{
    internal class Rental
    {
		public Movie Movie { get; }
		public int DaysRented { get; }
		public Rental(Movie movie, int daysRented)
		{
			Movie = movie;
			DaysRented = daysRented;
		}
	}
}
