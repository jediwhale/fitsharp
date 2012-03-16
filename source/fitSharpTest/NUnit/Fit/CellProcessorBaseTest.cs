// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class CellProcessorBaseTest {
        [Test] public void CachesParsedValue() {
            var cell = new CellTreeLeaf("<<symbol");
            var processor = Builder.CellProcessor();
            processor.Get<Symbols>().Save("symbol", "value");
            TypedValue result = processor.Parse(typeof (string), TypedValue.Void, cell);
            Assert.AreEqual("value", result.GetValue<string>());
            Assert.AreEqual(" value", cell.Value.GetAttribute(CellAttribute.InformationSuffix));
            processor.Parse(typeof (string), TypedValue.Void, cell);
            Assert.AreEqual(" value", cell.Value.GetAttribute(CellAttribute.InformationSuffix));
            Assert.AreEqual("value", result.GetValue<string>());
        }

        [Test] public void WrapsValue() {
            var processor = Builder.CellProcessor();
            var result = processor.Operate<WrapOperator>(new TypedValue("hi"));
            Assert.AreEqual("hi", result.ValueString);
        }
    }
}
