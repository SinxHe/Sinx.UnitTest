using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.EF
{
	public class EntryTests
	{
		private readonly EfDbContext _dbContext = new EfDbContext();
		private static readonly List<Course> Courses = new List<Course> {new Course {Name = "数学"}, new Course {Name = "语文"}};
		private readonly Student _studentWithForeign = new Student { Name = "Sinx", Courses = Courses};
		private void ResetDb()
		{
			using (var conn = new SqlConnection(ConnectStrings.DefaultConnectionString))
			{
				conn.Open();
				string sql = $@"
					IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[__MigrationHistory]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
					DROP TABLE [__MigrationHistory]
					IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[{nameof(Course)}]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
					DROP TABLE [{nameof(Course)}]
					IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[{nameof(Student)}]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
					DROP TABLE {nameof(Student)}";
				using (var cmd = new SqlCommand(sql,conn))
				{
					cmd.ExecuteNonQuery();
				}
				conn.Close();
			}
		}

		private static readonly Func<string, int> GetCount = sql =>
		{
			int count;
			using (var conn = new SqlConnection(ConnectStrings.DefaultConnectionString))
			{
				conn.Open();
				using (var cmd = new SqlCommand(sql, conn))
				{
					count = (int)cmd.ExecuteScalar();
				}
			}
			return count;
		};

		[Fact]
		public void ChangeProperty_State_Changed()
		{
			ResetDb();			
			var student = new Student
			{
				Name = "sinx",
				
			};
			var state = _dbContext.Entry(student).State;
			Assert.Equal(state, EntityState.Detached);
			_dbContext.Students.Add(student);
			state = _dbContext.Entry(student).State;
			Assert.Equal(state, EntityState.Added);
			_dbContext.SaveChanges();
			state = _dbContext.Entry(student).State;
			Assert.Equal(state, EntityState.Unchanged);
			student.Name = "sinxhe";
			state = _dbContext.Entry(student).State;
			Assert.Equal(state, EntityState.Modified);
		}

		[Fact]
		public void DeleteEntryWithNavigation_RemoveMethod_Success()
		{
			ResetDb();

			var state = _dbContext.Entry(_studentWithForeign).State;
			Assert.Equal(state, EntityState.Detached);
			state = _dbContext.Entry(_studentWithForeign.Courses.First()).State;
			Assert.Equal(state, EntityState.Detached);

			_dbContext.Students.Add(_studentWithForeign);
			state = _dbContext.Entry(_studentWithForeign).State;
			Assert.Equal(state, EntityState.Added);
			state = _dbContext.Entry(_studentWithForeign.Courses.First()).State;
			Assert.Equal(state, EntityState.Added);     // 调用方法的话, 导航实体会自动添加上相同的状态

			_dbContext.SaveChanges();
			state = _dbContext.Entry(_studentWithForeign).State;
			Assert.Equal(state, EntityState.Unchanged);
			state = _dbContext.Entry(_studentWithForeign.Courses.First()).State;
			Assert.Equal(state, EntityState.Unchanged);

			var courses = _studentWithForeign.Courses;
			var coursesCopy = courses.ToList();
			_dbContext.Students.Remove(_studentWithForeign);
			state = _dbContext.Entry(_studentWithForeign).State;
			Assert.Equal(state, EntityState.Deleted);
			//state = _dbContext.Entry(_studentWithForeign.Courses.First()).State;	// 这里调用Remove方法, 使得状态成了Deleted, 他的导航实体引用丢失, 但是应该是正确处理了导航实体的当前状态
			//Assert.Equal(state, EntityState.Deleted);
			state = _dbContext.Entry(coursesCopy.First()).State;	// 虽然主体对他的引用消失了, 但是可以用副本查看导航实体的当前状态
			Assert.Equal(state, EntityState.Deleted);	// 果然是Deleted了

			_dbContext.SaveChanges();
		}

		[Fact]
		public void DeleteEntryWithNavigation_UseStateFlag_Fail()
		{
			ResetDb();
			var state = _dbContext.Entry(_studentWithForeign).State;
			Assert.Equal(state, EntityState.Detached);
			_dbContext.Students.Add(_studentWithForeign);
			_dbContext.SaveChanges();
			var courses = _studentWithForeign.Courses;
			var coursesCopy = courses.ToList();
			//_dbContext.Entry<>
			_dbContext.Entry(_studentWithForeign).State = EntityState.Deleted;  // 这里赋值Deleted, 那么他的两个导航实体引用丢失, 但是并没有正确处理导航实体的当前状态, 导致删除的时候出错 -> 这里只是给主体状态变更为Deleted, 但是依赖体并没有赋值为Deleted, 所以EF不知道你要干什么, 到底是保留还是删除? http://stackoverflow.com/a/5540956/7573192
			state = _dbContext.Entry(_studentWithForeign).State;
			Assert.Equal(state, EntityState.Deleted);
			//state = _dbContext.Entry(_studentWithForeign.Courses.First()).State;	// 这里调用Remove方法, 使得状态成了Deleted, 他的导航实体引用丢失, 但是应该是正确处理了导航实体的当前状态
			//Assert.Equal(state, EntityState.Deleted);
			state = _dbContext.Entry(coursesCopy.First()).State;    // 虽然主体对他的引用消失了, 但是可以用副本查看导航实体的当前状态
			Assert.Equal(state, EntityState.Modified);	// Assert.Equal(state, EntityState.Deleted);   // 这里跟上边的Deleted不一样, 这里是把主体Student设置成了null, 换算到数据库就是StudentId为null, 但是导航键是不允许为null的, 所以出错
			Assert.Null(coursesCopy.First().Student);
			Assert.Throws<InvalidOperationException>(() => { _dbContext.SaveChanges(); });  // StudentId 不允许写入null
			state = _dbContext.Entry(coursesCopy.First()).State;
			Assert.Equal(state, EntityState.Modified);
			_dbContext.Entry(coursesCopy.First()).State = EntityState.Deleted;
			_dbContext.Entry(coursesCopy[1]).State = EntityState.Deleted;
			_dbContext.SaveChanges();
			string sql = $"SELECT COUNT(*) FROM {nameof(Student)}";
			Assert.Equal(0, GetCount(sql));
		}

		[Fact]
		public void DeleteEntityNavigation_ModifyEntityDirectory()
		{
			ResetDb();

			string sql = $"SELECT COUNT(*) FROM {nameof(Course)}";
			_dbContext.Students.Add(_studentWithForeign);
			_dbContext.SaveChanges();
			int count = GetCount(sql);
			Assert.NotEqual(0, count);

			//_studentWithForeign.Courses = new List<Course>(); Exception
			_studentWithForeign.Courses.ToList().ForEach(c => _dbContext.Entry(c).State = EntityState.Deleted);
			_dbContext.SaveChanges();
			count = GetCount(sql);
			Assert.Equal(0, count);
		}
	}
}
