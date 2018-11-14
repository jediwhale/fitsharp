// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture]
    public class StringTest {

        [Test]
        public void FindsTextBeforeValue() { Assert.AreEqual("some", "somebeforestuff".Before("before")); }

        [Test]
        public void FindsTextBeforeMultipleValues() { Assert.AreEqual("some", "somebeforestuff".Before(new [] {"befox", "before"})); }

        [Test]
        public void FindsShortestTextBeforeMultipleValues() { Assert.AreEqual("some", "somebeforebefoxstuff".Before(new [] {"befox", "before"})); }
        
    }
}