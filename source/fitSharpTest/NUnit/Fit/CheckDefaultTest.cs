// Copyright © 2013 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class CheckDefaultTest {
        [Test] public void ReturnsResultOfCheckMatching() {
            var check = new CheckDefault {Processor = Builder.CellProcessor()};
            var result = check.Check(CellOperationValue.Make(new TypedValue("stuff")), new CellTreeLeaf("stuff"));
            Assert.IsTrue(result.GetValue<bool>());
        }

        [Test] public void ReturnsResultOfCheckNotMatching() {
            var check = new CheckDefault {Processor = Builder.CellProcessor()};
            var result = check.Check(CellOperationValue.Make(new TypedValue("other")), new CellTreeLeaf("stuff"));
            Assert.IsFalse(result.GetValue<bool>());
        }
    }
}
