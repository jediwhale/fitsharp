using System;
using System.Collections.Generic;
using System.Text;

using dbfit.util;
using NUnit.Framework;

namespace dbfit {
	[TestFixture]
	public class NameNormaliserTest {

		[Test]
		public void CheckNormaliseName() {
			Assert.AreEqual("dbtest", NameNormaliser.NormaliseName("dbtest?"));
			Assert.AreEqual("dbtest", NameNormaliser.NormaliseName("db test"));
			Assert.AreEqual("dbtest", NameNormaliser.NormaliseName("db test?"));
			Assert.AreEqual("db.test", NameNormaliser.NormaliseName("db.test"));
			Assert.AreEqual("db_test", NameNormaliser.NormaliseName("db_test"));
			Assert.AreEqual("dbtest", NameNormaliser.NormaliseName("DbTeSt"));
		}
	}
}
