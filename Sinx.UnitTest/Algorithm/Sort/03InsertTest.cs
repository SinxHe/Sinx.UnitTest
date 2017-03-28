using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.Algorithm.Sort
{
	// ReSharper disable once InconsistentNaming
	public class _03InsertTest
	{
		[Fact]
		public void Insert()
		{
			var ar = Vars.Ar.ToList();
			var arSorted = Vars.ArSorted;
			for (int i = 0; i < ar.Count; i++)
			{
				for (int j = 0; j <= i; j++)    // = for 0 situation
				{
					if (ar[i] <= ar[j])
					{
						var t = ar[i];
						ar.RemoveAt(i);
						ar.Insert(j, t);
					}
				}
			}
			Assert.Equal(ar, arSorted);
		}
	}
}
