using System;
using System.Collections.Generic;
using System.Text;

using dbfit.util;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace dbfit {
	[TestFixture]
	public class NameNormaliserTest {

		[Test]
		public void CheckNormaliseName() {
			ClassicAssert.AreEqual("dbtest", NameNormaliser.NormaliseName("dbtest?"));
			ClassicAssert.AreEqual("dbtest", NameNormaliser.NormaliseName("db test"));
			ClassicAssert.AreEqual("dbtest", NameNormaliser.NormaliseName("db test?"));
			ClassicAssert.AreEqual("db.test", NameNormaliser.NormaliseName("db.test"));
			ClassicAssert.AreEqual("db_test", NameNormaliser.NormaliseName("db_test"));
			ClassicAssert.AreEqual("dbtest", NameNormaliser.NormaliseName("DbTeSt"));
		}
	}
}
