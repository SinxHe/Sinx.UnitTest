using System;
using System.Collections.Generic;
using System.Text;

namespace Sinx.Refactor.Tests._01_MovieRental_Simple
{
    internal class Movie
    {
	    public const int CHILDRENS = 2;
	    public const int REGULAR = 0;
	    public const int NEW_RELEASE = 1;


		public int PriceCode { get; set; }

		public string Title { get; }

		public Movie(string title, int priceCode)
		{
			PriceCode = priceCode;
			Title = title;
		}
    }
}
