using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Xunit;
using Dapper;

namespace Sinx.UnitTest.Persistence.MySql
{
	public class CrudTest
	{
		private const string ConnString = "server=neter.me; port=3306;user id=SinxHe; password=SinxHe;database=test";
		#region Table

		//[Fact]
		public void MySql_Table_CreateAndDelete()
		{
			var tableName = "temp" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			var sql = $@"CREATE TABLE {tableName}(
				Id INT NOT NULL AUTO_INCREMENT,
				PRIMARY KEY ( Id ),
				Name VARCHAR(100) NOT NULL,
				CreateTime DATE
			   );";
			using (IDbConnection conn = new MySqlConnection(ConnString))
			{
				var number = conn.ExecuteAsync(sql).Result;
			}

			sql = $"SELECT COUNT(*) FROM {tableName}";

			sql = $"DROP TABLE {tableName}";
			using (IDbConnection conn = new MySqlConnection(ConnString))
			{
				var number = conn.ExecuteAsync(sql).Result;
			}
		}

		#endregion
	}
}
