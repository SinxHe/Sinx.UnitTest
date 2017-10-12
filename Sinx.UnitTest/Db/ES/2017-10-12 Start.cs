using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Xunit;

namespace Sinx.UnitTest.Db.ES
{
	public class _2017_10_12_Start
	{
		[Fact]
		public void ES_CreateTestDataSettings()
		{

		}

		[Fact]
		public void ES_CreateTestData()
		{
			const string header = "post /index/student";
			var json = header + Environment.NewLine + string.Join(
				Environment.NewLine,
				FakeStudent.GetStudents()
					.Select((s, i) =>
					{
						var prefix = "{'index': { '_id': {i} }}"
							.Replace("'", "\"")
							.Replace("{i}", (i + 1).ToString());
						return prefix + Environment.NewLine + JsonConvert.SerializeObject(s);
					}));
			//post /index/student
			//{"index": { "_id": 1 }}
			//{"Id":1,"GuidId":"836ddee8-4b5a-4ee1-93c8-d9df11c87d98","Age":23,"Name":"何士雄","Info":{"Id":1,"StudentId":1,"Address":"河北省保定市定州市"}}
			//{"index": { "_id": 2 }}
			//{"Id":2,"GuidId":"2db62d6c-4e53-43bc-99a7-102b3e999944","Age":22,"Name":"王兴宇","Info":{"Id":2,"StudentId":2,"Address":"河北省廊坊市"}}
			//{"index": { "_id": 3 }}
			//{"Id":3,"GuidId":"a5936e00-a038-43bd-b138-c9571bbbe87d","Age":23,"Name":"冯浩","Info":null}
			//{"index": { "_id": 4 }}
			//{"Id":4,"GuidId":"7f73fe84-6341-452c-990d-415930124af4","Age":23,"Name":"李宁超","Info":null}
			//{"index": { "_id": 5 }}
			//{"Id":5,"GuidId":"844ef97d-d563-4db3-a32b-98957ffe8182","Age":24,"Name":"杜鹏","Info":{"Id":4,"StudentId":5,"Address":"辽宁省葫芦岛市"}}
			//{"index": { "_id": 6 }}
			//{"Id":6,"GuidId":"7967145c-f186-420d-8791-8e2a527db3e1","Age":23,"Name":"景建威","Info":{"Id":3,"StudentId":6,"Address":"河北省邢台市"}}
		}
	}
}
