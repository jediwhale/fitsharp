// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    public class TestOperator : CellOperator, ParseOperator<Cell> {
        public bool CanParse(System.Type type, TypedValue instance, Tree<Cell> parameters) {
            return true;
        }

        public TypedValue Parse(System.Type type, TypedValue instance, Tree<Cell> parameters) {
            return TypedValue.Void;
        }
    }

    [TestFixture] public class CellOperatorsTest {
        [Test] public void LoadsOperatorByName() {
            var operators = new CellOperators();
            operators.Add("ParseDate");
            operators.Do<ParseOperator<Cell>>(o => (o is ParseDate), o => {});
            ClassicAssert.IsTrue(true, "no exception");
        }

        [Test] public void FindsWrapOperator() {
            var operators = new CellOperators();
            var result = operators.FindOperator<WrapOperator>(new object[] {null});
            ClassicAssert.AreEqual(typeof(WrapDefault), result.GetType());
        }

        [Test] public void AddedFirstGetsExecutedFirst() {
            var operators = new CellOperators();
            var registered = operators.AddFirst("fitsharp.Test.NUnit.Fit.TestOperator");

            ParseOperator<Cell> executed = null;
            operators.Do<ParseOperator<Cell>>(o => true, o => { executed = o; });

            ClassicAssert.AreSame(registered, executed);
        }
    }
}
