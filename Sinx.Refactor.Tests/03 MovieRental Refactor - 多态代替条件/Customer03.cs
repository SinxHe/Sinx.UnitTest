using System;
using System.Collections.Generic;
using System.Linq;

namespace Sinx.Refactor.Tests
{
	internal class Customer03
	{
		public string Name { get; }
		private readonly List<Rental03> _rentals;

		public Customer03(string name)
		{
			Name = name;
			_rentals = new List<Rental03>();
		}

		public void AddRental(Rental03 rental)
		{
			_rentals.Add(rental);
		}

		/// <summary>
		/// NOTICE: 做的事情太多, 很多事情是其他类应该完成的
		/// NOTICE: 变化: 添加新的计费规则, 积分规则, 输出方式
		/// </summary>
		public string Statement()
		{
			var result = "Rental Record for " + Name + Environment.NewLine;
			foreach (var rental in _rentals)
			{
				result += "\t" + rental.Movie.Title +
							"\t" + rental.GetCharge() + Environment.NewLine;
			}

			result += $"Amount owed is {GetTotalCharge()}{Environment.NewLine}";
			result += $"You earned {GetTotalFrequentRenterPoints()} frequent renter points";
			return result;
		}

		private double GetTotalCharge()
		{
			return _rentals.Sum(rental => rental.GetCharge());
		}

		private double GetTotalFrequentRenterPoints()
		{
			return _rentals.Sum(rental => rental.GetFrequentRenterPoints());
		}

		/// <summary>
		/// 这里只使用了Rental的信息没有使用Consumer的信息
		/// 这里的代码是否放错了位置????
		/// </summary>
		private double AmountForTemp(Rental03 rental)
		{
			double result = 0;
			switch (rental.Movie.PriceCode)
			{
				case Movie03.REGULAR:
					result += 2;
					if (rental.DaysRented > 2)
					{
						result += (rental.DaysRented - 2) * 1.5;
					}
					break;
				case Movie03.NEW_RELEASE:
					result += rental.DaysRented * 3;
					break;
				case Movie03.CHILDRENS:
					result += 1.5;
					if (rental.DaysRented > 3)
					{
						result += (rental.DaysRented - 3) * 1.5;
					}
					break;
			}
			return result;
		}
	}
}
