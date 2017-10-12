using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinx.UnitTest.Db.ES
{
	public class Student
	{
		public int Id { get; set; }
		public string GuidId { get; set; }
		public int Age { get; set; }
		public string Name { get; set; }
		public Info Info { get; set; }
	}
}
