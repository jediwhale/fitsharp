// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using NUnit.Framework;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class FlowRowTest {

        [Test]
        public void ExecutesRowMethod() {
            var sut = new SampleClass();
            Evaluate(new CellTree("countby", "2"), sut);
            Assert.AreEqual(2, sut.Count);
        }

        [Test]
        public void ExecutesFlowKeyword() {
            var row = new CellTree("akeyword");
            Evaluate(row, null);
            Assert.AreEqual("somevalue", TestInvokeSpecialAction.Field);
        }

        static void Evaluate(Tree<Cell> row, object systemUnderTest) {
            var processor = Builder.CellProcessor();
            processor.CallStack.DomainAdapter = new TypedValue(new DefaultFlowInterpreter(systemUnderTest));
            processor.AddOperator(new TestInvokeSpecialAction(), 2);
            row.Evaluate(processor);
        }

        class TestInvokeSpecialAction: CellOperator, InvokeSpecialOperator  {
            public bool CanInvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
                return true;
            }

            public TypedValue InvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
                if (memberName.Name != "akeyword") return TypedValue.MakeInvalid(new ApplicationException("argh"));
                Field = "somevalue";
                return TypedValue.Void;
            }

            public static string Field;
        }
    }
}
