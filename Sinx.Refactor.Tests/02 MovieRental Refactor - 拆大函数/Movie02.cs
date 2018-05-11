namespace Sinx.Refactor.Tests
{
    internal class Movie02
    {
	    public const int CHILDRENS = 2;
	    public const int REGULAR = 0;
	    public const int NEW_RELEASE = 1;


		public int PriceCode { get; set; }

		public string Title { get; }

		public Movie02(string title, int priceCode)
		{
			PriceCode = priceCode;
			Title = title;
		}
    }
}
