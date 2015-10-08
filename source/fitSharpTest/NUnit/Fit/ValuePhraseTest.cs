// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using fitSharp.Test.Double;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class ValuePhraseTest {

        [Test]
        public void ReturnsSymbolValue() {
            var processor = Builder.CellProcessor();
            processor.Get<Symbols>().Save("mysymbol", 123);
            Assert.AreEqual(123, new ValuePhrase(new CellTree("keyword", "mysymbol")).Evaluate(processor));
        }

        [Test]
        public void ReturnsMethodValue() {
            var processor = Builder.CellProcessor();
            var interpreter = new DefaultFlowInterpreter(new SampleClass());
            processor.CallStack.DomainAdapter = new TypedValue(interpreter);
            Assert.AreEqual("samplereturn", new ValuePhrase(new CellTree("keyword", "methodnoparms")).Evaluate(processor));
        }
    }
}
