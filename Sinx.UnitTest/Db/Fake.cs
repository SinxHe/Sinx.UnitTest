using System.Data;
using MySql.Data.MySqlClient;

namespace Sinx.UnitTest.Db
{
	public static class Fake
	{
		public static IDbConnection MySqlConnection => new MySqlConnection("Server=neter.me;Port=3305;Database=TestDB;Uid=SinxHe;Pwd=SinxHe*#7370#;");
	}
}
