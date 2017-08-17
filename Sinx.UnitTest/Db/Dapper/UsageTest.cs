using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Xunit;

namespace Sinx.UnitTest.Db.Dapper
{
	public class UsageTest
	{
		[Fact]
		public async Task Dapper_GetEntity_WithTwoDTO()
		{
			var db = Fake.MySqlConnection;
			var m = await db.QueryAsync<Department>(
			@"SELECT * FROM employees.departments D
			LEFT JOIN employees.dept_emp DE ON DE.dept_no = D.dept_no
			LEFT JOIN employees.employees E ON E.emp_no = DE.emp_no");

		}
	}

	class Department
	{
		public string dept_no { get; set; }
		public string dept_name { get; set; }
		public List<Employee> employees { get; set; }
	}

	class Employee
	{
		public int emp_no { get; set; }
		public string first_name { get; set; }
		public string last_name { get; set; }
		public string gender { get; set; }
		public DateTime birth_date { get; set; }
		public DateTime hire_date { get; set; }
	}
}
