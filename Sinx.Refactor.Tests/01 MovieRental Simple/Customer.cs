using System;
using System.Collections.Generic;

namespace Sinx.Refactor.Tests._01_MovieRental_Simple
{
	internal class Customer
	{
		public string Name { get; }
		private readonly List<Rental> _rentals;

		public Customer(string name)
		{
			Name = name;
			_rentals = new List<Rental>();
		}

		public void AddRental(Rental rental)
		{
			_rentals.Add(rental);
		}

		/// <summary>
		/// NOTICE: 做的事情太多, 很多事情是其他类应该完成的
		/// NOTICE: 变化: 添加新的计费规则, 积分规则, 输出方式
		/// </summary>
		public string Statement()
		{
			double totalAmount = 0;
			int frequentRenterPoints = 0;
			var result = "Rental Record for " + Name + Environment.NewLine;
			foreach (var rental in _rentals)
			{
				double thisAmount = 0;
				switch (rental.Movie.PriceCode)
				{
					case Movie.REGULAR:
						thisAmount += 2;
						if (rental.DaysRented > 2)
						{
							thisAmount += (rental.DaysRented - 2) * 1.5;
						}
						break;
					case Movie.NEW_RELEASE:
						thisAmount += rental.DaysRented * 3;
						break;
					case Movie.CHILDRENS:
						thisAmount += 1.5;
						if (rental.DaysRented > 3)
						{
							thisAmount += (rental.DaysRented - 3) * 1.5;
						}
						break;
				}

				frequentRenterPoints++;
				if (rental.Movie.PriceCode == Movie.NEW_RELEASE && rental.DaysRented > 1)
				{
					frequentRenterPoints++;
				}

				result += "\t" + rental.Movie.Title +
							"\t" + thisAmount + Environment.NewLine;
				totalAmount += thisAmount;
			}

			result += $"Amount owed is {totalAmount}{Environment.NewLine}";
			result += $"You earned {frequentRenterPoints} frequent renter points";
			return result;
		}
	}
}
