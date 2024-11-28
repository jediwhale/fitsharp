// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Globalization;
using System.Threading;
using fitSharp.Machine.Engine;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] [SetCulture("en-US")] public class CultureTest {
        [Test] public void SetUpSetsCulture() {
            var culture = new Culture {Name = "af-ZA"};
            culture.SetUp();
            ClassicAssert.AreEqual("af-ZA", Thread.CurrentThread.CurrentCulture.Name);
        }

        [Test] public void TearDownRestoresCulture() {
            var culture = new Culture {Name = "af-ZA"};
            culture.SetUp();
            culture.TearDown();
            ClassicAssert.AreEqual("en-US", Thread.CurrentThread.CurrentCulture.Name);
        }

        [Test] public void InvariantCultureIsSet() {
            var culture = new Culture {Name = "invariant"};
            culture.SetUp();
            ClassicAssert.AreEqual(CultureInfo.InvariantCulture, Thread.CurrentThread.CurrentCulture);
        }
    }
}
