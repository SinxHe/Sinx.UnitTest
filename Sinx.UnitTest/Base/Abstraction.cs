using System;
namespace Sinx.UnitTest.Base
{
	#region 使用

	public class Cup1
	{
		public int Capacity { get; set; }

		public void Add()
		{
		}

		public void Reduce()
		{
		}
	}

	#endregion

	#region 观赏

	public class Cup2
	{
		public string Material { get; set; }
		public string Color { get; set; }
		public DateTime CreateTime { get; set; }
	}

	#endregion

	#region 生产
	public class Cup3
	{
		public double Cost { get; set; }
		public double Price { get; set; }
	}
	#endregion
}
