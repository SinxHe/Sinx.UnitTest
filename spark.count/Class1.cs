using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Spark.CSharp.Core;
using Xunit;

namespace Sinx.UnitTest.Hadoop.Spark
{
	public class SparkTests
	{
		private SparkContext sparkContext = new SparkContext("master", "appname");
		public void WordCount()
		{
			var lines = sparkContext.TextFile(@"file:///usr/hdp/current/spark-client/bin/derby.log");
			var words = lines.FlatMap(s => s.Split(' '));
			var wordCounts = words.Map(w => new Tuple<string, int>(w.Trim(), 1))
								  .ReduceByKey((x, y) => x + y)
								  ;
			var wordCountCollection = wordCounts.Collect();
			words.SaveAsTextFile(@"file:///usr/hdp/current/spark-client/bin/derby.result.log");
		}
	}
}
