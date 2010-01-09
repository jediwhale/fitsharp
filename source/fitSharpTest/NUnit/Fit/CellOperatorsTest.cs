// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class CellOperatorsTest {
        [Test] public void LoadsOperatorByName() {
            var operators = new CellOperators();
            operators.Add("ParseDate");
            operators.Do<ParseOperator<Cell>>(o => (o is ParseDate), o => {});
            Assert.IsTrue(true, "no exception");
        }
    }
}
