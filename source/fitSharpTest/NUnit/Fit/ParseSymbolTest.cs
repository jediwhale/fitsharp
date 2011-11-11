// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using fitSharp.Test.Double.Fit;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseSymbolTest {
        [Test] public void AddsSymbolValueToCellAttributes() {
            ParseCell("<<symbol", "symbol");
        }

        [Test] public void SymbolNameIsTrimmed() {
            ParseCell("<<symbol\n", " symbol ");
        }

        private static void ParseCell(string cellContent, string symbolName) {
            var cell = new CellTreeLeaf(cellContent);
            var processor = Builder.CellProcessor();
            processor.Get<Symbols>().Save(symbolName, "value");
            new ParseSymbol{Processor = processor}.Parse(typeof (string), TypedValue.Void, cell);
            Assert.AreEqual(" value", cell.Value.GetAttribute(CellAttribute.InformationSuffix));
        }
    }
}
