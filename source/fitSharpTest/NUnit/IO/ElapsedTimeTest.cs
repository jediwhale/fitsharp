// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.IO;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.IO {
    [TestFixture] public class ElapsedTimeTest {
        [Test] public void ShortTimeIsShown() {
            Clock.Instance = TestClock.Instance;
            TestClock.Instance.UtcNow = new DateTime(2009, 1, 2, 3, 4, 5, 6);
            var elapsed = new ElapsedTime();
            TestClock.Instance.UtcNow = new DateTime(2009, 1, 2, 3, 4, 5, 7);
            Assert.AreEqual("00:00:00.001", elapsed.ToString());
        }

        [Test] public void LongTimeIsShown() {
            Clock.Instance = TestClock.Instance;
            TestClock.Instance.UtcNow = new DateTime(2009, 1, 2, 3, 4, 5, 6);
            var elapsed = new ElapsedTime();
            TestClock.Instance.UtcNow = new DateTime(2010, 1, 2, 6, 4, 5, 7);
            Assert.AreEqual("8763:00:00.001", elapsed.ToString());
        }
    }
}
