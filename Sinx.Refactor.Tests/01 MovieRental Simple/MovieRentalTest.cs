using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sinx.Refactor.Tests._01_MovieRental_Simple
{
    public class MovieRentalTest
    {
		[Fact]
	    public void MovieRental_Start()
	    {
		    var movie = new Movie("终结者", 2);
		    var rental = new Rental(movie, 3);
		    var customer = new Customer("sinx");
		    customer.AddRental(rental);
		    var desc = customer.Statement();
		    const string expect = @"Rental Record for sinx
	终结者	1.5
Amount owed is 1.5
You earned 1 frequent renter points";
		    Assert.Equal(expect, desc);
	    }
    }
}
