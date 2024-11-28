// Copyright © 2015 Syterra Software Inc. All rights reserved.
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
    [TestFixture] public class ParseSymbolTest : ParseOperatorTest<ParseSymbol> {
        [Test] public void CanParse() {
            ClassicAssert.IsTrue(CanParse<string>("<<symbol"), "<<symbol");
            ClassicAssert.IsFalse(CanParse<string>("no<<symbol"), "no<<symbol");
            ClassicAssert.IsTrue(CanParse<string>("\t<<symbol\r\n"), "\t<<symbol\r\n");
            ClassicAssert.IsFalse(CanParse<string>("<nosymbol"), "<nosymbol");
            ClassicAssert.IsFalse(CanParse<string>("nosymbol<<"), "nosymbol<<");
            ClassicAssert.IsFalse(CanParse<string>("<<symbol1,other"), "array");
        }

        [Test] public void Parse() {
            Parser.Processor.Get<Symbols>().Save("symbol", "symbol value");

            ClassicAssert.AreEqual("symbol value", Parse<string>("<<symbol"), "<<symbol");
            ClassicAssert.AreEqual("symbol value", Parse<string>("\t<<symbol\r\n"), "\t<<symbol\r\n");
        }

        [Test] public void ParsesTypeFromString() {
            Parser.Processor.Get<Symbols>().Save("symbol", 123.45d.ToString());
            ClassicAssert.AreEqual(123.45d, ParseAs<double>("<<symbol"));
        }

        [Test] public void ParseAddsInformationSuffix() {
            Parser.Processor.Get<Symbols>().Save("symbol", "value");

            var cell = new CellTreeLeaf("<<symbol");
            Parser.Parse(typeof(string), TypedValue.Void, cell);
            ClassicAssert.AreEqual(" value", cell.Value.GetAttribute(CellAttribute.InformationSuffix));
        }
    }
}
