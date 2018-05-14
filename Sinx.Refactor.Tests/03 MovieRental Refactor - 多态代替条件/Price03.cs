namespace Sinx.Refactor.Tests
{
	/// <summary>
	/// 这里使用State模式来替换不同影片类型对应的价格
	/// 之所以不使用Movie的多态是因为Movie在生命周期内是类型是有可能变的
	/// </summary>
    public  abstract class Price03
	{
		public abstract int GetPriceCode();

		public abstract double GetCharge(int daysRenged);

		public virtual int GetFrequentRenterPoints(int daysRented)
		{
			return 1;
		}
	}
}
