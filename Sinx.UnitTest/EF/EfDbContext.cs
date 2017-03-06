using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinx.UnitTest.EF
{
	public class EfDbContext : DbContext
	{
		public EfDbContext() : base(ConnectStrings.DefaultConnectionString)
		{
			//Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EfDbContext>());
			this.Database.Log = s => Debug.WriteLine(s);    // 开启日志
		}
		public DbSet<Student> Students { get; set; }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(Course.Configuration);
			modelBuilder.Configurations.Add(Student.Configuration);
		}
	}
}
