// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Reflection;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class TestStatusTest {
        [Test] public void MarkWithAbandonSuiteAbandonsSuite() {
            AssertAbandonsSuite(new AbandonSuiteException());
        }

        [Test] public void MarkWithNestedAbandonSuiteAbandonsSuite() {
            AssertAbandonsSuite(new TargetInvocationException(new AbandonSuiteException()));
        }

        static void AssertAbandonsSuite(Exception exception) {
            var status = new fitSharp.Fit.Model.TestStatus();
            Assert.IsFalse(status.SuiteIsAbandoned);
            try {
                status.MarkException(new CellBase("test"), exception);
            }
            catch (AbandonSuiteException) {}
            Assert.IsTrue(status.SuiteIsAbandoned);
        }
    }
}
