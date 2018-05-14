namespace Sinx.Refactor.Tests
{
    internal class Movie03
    {
	    public const int CHILDRENS = 2;
	    public const int REGULAR = 0;
	    public const int NEW_RELEASE = 1;

	    private Price03 _priceCode;

	    public int PriceCode
	    {
		    get => _priceCode.GetPriceCode();
		    set
		    {
			    switch (value)
			    {
				    // NOTICE: 不要在别人的对象属性上使用SWITCH
				    case Movie03.REGULAR:
					    _priceCode = new RegularPrice03();
					    break;
				    case Movie03.NEW_RELEASE:
					    _priceCode = new NewReleasePrice03();
					    break;
				    case Movie03.CHILDRENS:
					    _priceCode = new ChildrensPrice03();
					    break;
					default:
						throw new System.ArgumentException();
			    }
			}
	    }

		public string Title { get; }

		public Movie03(string title, int priceCode)
		{
			PriceCode = priceCode;
			Title = title;
		}

		// NOTICE: 这里用到Rental类的DaysRented和Movie的类型
		// 之所以传递DaysRented而不是Movie的类型是因为Movie类型
		// 是一个很可能增加的类型, 要控制此改变的影响
	    public double GetCharge(int daysRenged)
	    {
		    return _priceCode.GetCharge(daysRenged);
	    }

	    public int GetFrequentRenterPoints(int daysRented)
	    {
		    return _priceCode.GetFrequentRenterPoints(daysRented);
	    }
    }
}
