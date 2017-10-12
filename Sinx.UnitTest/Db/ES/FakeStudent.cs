using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinx.UnitTest.Db.ES
{
	public class FakeStudent
	{
		public static IEnumerable<Student> GetStudents()
		{
			var students = new List<Student>
			{
				new Student {Id = 1, GuidId = GetGuid(), Name = "何士雄", Age = 23},
				new Student {Id = 1, GuidId = GetGuid(), Name = "何", Age = 23},
				new Student {Id = 1, GuidId = GetGuid(), Name = "Da Xiong Ge ge", Age = 23},
				new Student {Id = 1, GuidId = GetGuid(), Name = "xiong", Age = 23},
				new Student {Id = 2, GuidId = GetGuid(), Name = "王兴宇", Age = 22},
				new Student {Id = 3, GuidId = GetGuid(), Name = "冯浩", Age = 23},
				new Student {Id = 4, GuidId = GetGuid(), Name = "李宁超", Age = 23},
				new Student {Id = 5, GuidId = GetGuid(), Name = "杜鹏", Age = 24},
				new Student {Id = 6, GuidId = GetGuid(), Name = "景建威", Age = 23}
			};

			var infos = new List<Info>
			{
				new Info {Id = 1, StudentId = 1, Address = "河北省保定市定州市"},
				new Info {Id = 2, StudentId = 2, Address = "河北省廊坊市"},
				new Info {Id = 3, StudentId = 6, Address = "河北省邢台市"},
				new Info {Id = 4, StudentId = 5, Address = "辽宁省葫芦岛市"}
			};

			//return
			//	from s in students
			//	join i in infos on s.Id equals i.StudentId into g
			//	from iTemp in g.DefaultIfEmpty()
			//	select new Student
			//	{
			//		Age = s.Age,
			//		GuidId = s.GuidId,
			//		Id = s.Id,
			//		Name = s.Name,
			//		Info = iTemp
			//	};
			return students.Select(s =>
			{
				s.Info = infos.FirstOrDefault(i => i.StudentId == s.Id);
				return s;
			});
		}

		private static string GetGuid()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
