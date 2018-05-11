using Xunit;

namespace Sinx.Refactor.Tests
{
    public class MovieRentalTest02
    {
		[Fact]
	    public void MovieRental_Start()
	    {
		    var movie = new Movie02("终结者", 2);
		    var rental = new Rental02(movie, 3);
		    var customer = new Customer02("sinx");
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
