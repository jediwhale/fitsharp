// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.IO;
using fitSharp.Samples;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.IO {
    [TestFixture] public class ElapsedTimeTest {
        [Test] public void ShortTimeIsShown() {
            Clock.Instance = TestClock.Instance;
            var elapsed = new ElapsedTime();
            TestClock.Instance.Elapsed = new TimeSpan(0);
            ClassicAssert.AreEqual("0.001", elapsed.ToString());
        }

        [Test] public void LongTimeIsShown() {
            Clock.Instance = TestClock.Instance;
            var elapsed = new ElapsedTime();
            TestClock.Instance.Elapsed = new TimeSpan(0, 8763, 0, 0, 0);
            ClassicAssert.AreEqual("8763:00:00.001", elapsed.ToString());
        }
    }
}
