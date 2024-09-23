// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class CellProcessorBaseTest {
        [Test] public void CachesParsedValue() {
            var cell = new CellTreeLeaf("<<symbol");
            var processor = Builder.CellProcessor();
            processor.Get<Symbols>().Save("symbol", "value");
            TypedValue result = processor.Parse(typeof (string), TypedValue.Void, cell);
            ClassicAssert.AreEqual("value", result.GetValue<string>());
            ClassicAssert.AreEqual(" value", cell.Value.GetAttribute(CellAttribute.InformationSuffix));
            processor.Parse(typeof (string), TypedValue.Void, cell);
            ClassicAssert.AreEqual(" value", cell.Value.GetAttribute(CellAttribute.InformationSuffix));
            ClassicAssert.AreEqual("value", result.GetValue<string>());
        }

        [Test] public void WrapsValue() {
            var processor = Builder.CellProcessor();
            var result = processor.Operate<WrapOperator>(new TypedValue("hi"));
            ClassicAssert.AreEqual("hi", result.ValueString);
        }

        [Test] public void UsesCreateOperator() {
            var processor = Builder.CellProcessor();
            processor.AddOperator(new TestCreateOperator());
            var result = processor.Create("testname");
            ClassicAssert.AreEqual("mytestname", result.GetValueAs<string>());
        }

        class TestCreateOperator: CellOperator, CreateOperator<Cell> {
            public bool CanCreate(NameMatcher memberName, Tree<Cell> parameters) {
                return memberName.Matches("testname");
            }

            public TypedValue Create(NameMatcher memberName, Tree<Cell> parameters) {
                return new TypedValue("mytestname");
            }
        }
    }
}
