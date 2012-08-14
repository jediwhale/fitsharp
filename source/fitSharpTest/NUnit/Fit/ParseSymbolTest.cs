// Copyright © 2011 Syterra Software Inc. All rights reserved.
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

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseSymbolTest : ParseOperatorTest<ParseSymbol> {
        [Test] public void CanParse() {
            Assert.IsTrue(CanParse<string>("<<symbol"), "<<symbol");
            Assert.IsTrue(CanParse<string>("\t<<symbol\r\n"), "\t<<symbol\r\n");
            Assert.IsFalse(CanParse<string>("<nosymbol"), "<nosymbol");
            Assert.IsFalse(CanParse<string>("nosymbol<<"), "nosymbol<<");
        }

        [Test] public void Parse() {
            Parser.Processor.Get<Symbols>().Save("symbol", "symbol value");

            Assert.AreEqual("symbol value", Parse<string>("<<symbol"), "<<symbol");
            Assert.AreEqual("symbol value", Parse<string>("\t<<symbol\r\n"), "\t<<symbol\r\n");
        }

        [Test] public void ParseAddsInformationSuffix() {
            Parser.Processor.Get<Symbols>().Save("symbol", "value");

            var cell = new CellTreeLeaf("<<symbol");
            Parser.Parse(typeof(string), TypedValue.Void, cell);
            Assert.AreEqual(" value", cell.Value.GetAttribute(CellAttribute.InformationSuffix));
        }
    }
}
