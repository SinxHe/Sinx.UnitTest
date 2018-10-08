using System.Data;
using MySql.Data.MySqlClient;

namespace Sinx.UnitTest.Db
{
	public static class Fake
	{
		public static IDbConnection MySqlConnection => new MySqlConnection("{connection_string_place_holder}");
	}
}
