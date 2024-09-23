using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace dbfit {
	[TestFixture]
	public class OracleEnvironmentTest {
		private OracleEnvironment oe=new OracleEnvironment();
		[Test]
		public void CheckEmptyParams() {
			ClassicAssert.AreEqual(0,oe.ExtractParamNames("select * from dual").Length);			
		}
		[Test]
		public void CheckSingleParam() {
			ClassicAssert.AreEqual(new string[]{"mydate"}, oe.ExtractParamNames("select * from dual where sysdate<:mydate"));
		}
		[Test]
		public void CheckMultipleParams() {
			string[] paramnames=oe.ExtractParamNames("select :myname as zeka from dual where sysdate<:mydate");
			ClassicAssert.AreEqual(2,paramnames.Length);
            ClassicAssert.Contains("mydate", paramnames);
            ClassicAssert.Contains("myname", paramnames);
		}
		[Test]
		public void CheckMultipleParamsRecurring() {
			string[] paramnames = oe.ExtractParamNames("select :myname,length(:myname) as l, :myname || :mydate as zeka2 from dual where sysdate<:mydate");
			ClassicAssert.AreEqual(2, paramnames.Length);
            ClassicAssert.Contains("mydate", paramnames);
            ClassicAssert.Contains("myname", paramnames);
		}
		[Test]
		public void CheckUnderscore() {
			ClassicAssert.AreEqual(new string[] { "my_date" }, oe.ExtractParamNames("select * from dual where sysdate<:my_date"));
		}
	}
}
