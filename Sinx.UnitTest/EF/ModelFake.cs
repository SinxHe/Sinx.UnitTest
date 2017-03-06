using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinx.UnitTest.EF
{
	public class Student
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public List<Course> Courses { get; set; }

		public static readonly EfTypeConfiguration Configuration = new EfTypeConfiguration();

		public class EfTypeConfiguration : EntityTypeConfiguration<Student>
		{
			public EfTypeConfiguration()
			{
				this.HasKey(e => e.Id)
					.ToTable(nameof(Student));
			}
		}
	}

	public class Course
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public int StudentId { get; set; }
		public Student Student { get; set; }

		public static EfTypeConfiguration Configuration = new EfTypeConfiguration();

		public class EfTypeConfiguration : EntityTypeConfiguration<Course>
		{
			public EfTypeConfiguration()
			{
				this.HasKey(e => e.Id)
					.HasRequired(e => e.Student)
					.WithMany(e => e.Courses)
					.HasForeignKey(e => e.StudentId);
				this.ToTable(nameof(Course));
			}
		}
	}
}
