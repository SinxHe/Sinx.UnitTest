using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using MySql.Data.MySqlClient;

namespace Sinx.UnitTest.Db.MySql
{
	public class ProcedureTests
	{
		private const string ConnString = "{connection_string_place_holder}";
		private readonly IDbConnection _db = new MySqlConnection(ConnString);
		[Fact]
		public void Procedure_Create()
		{
			var d = new DynamicParameters();
			d.Add("@avgage", dbType: DbType.Int32, direction: ParameterDirection.Output);
			_db.Query("GET_AvgAge", d, commandType: CommandType.StoredProcedure);
			var age = d.Get<int>("@avgage");
		}
	}
}
