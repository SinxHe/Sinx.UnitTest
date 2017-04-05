using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;
using Xunit;

namespace Sinx.UnitTest.Thrift
{
	public class ThriftTest
	{

		[Fact]
		public void Thrift_Client()
		{
			//var socket = new TSocket("10.1.1.26", 4142);
			//var transport = new TFramedTransport(socket);   // transport 传输
			//var protocol = new TCompactProtocol(transport); // protocol 协议
			//transport.Open();
			//var dic = new Dictionary<string, string>
			//{
			//	{ "timestamp", ((long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds).ToString()},
			//	{ "appId", "1"}
			//};
			//using (var client = new ThriftSourceProtocol.Client(protocol))
			//{
				
			//	var @event = new ThriftFlumeEvent(dic, Encoding.UTF8.GetBytes("helloword"));
			//	client.append(@event);
			//}
			
			//transport.Close();
		}

		private enum LogPriority
		{
			Fatal = 50000,
			Error = 40000,
			Warn = 30000,
			Info = 20000,
			Debug = 10000,
			Trace = int.MinValue,
		}
	}
}
