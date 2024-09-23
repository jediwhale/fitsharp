// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Parser;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture]
    public class ExcelCellTest {
        [TestCase("A1", 0, 0)]
        [TestCase("B5", 1, 4)]
        [TestCase("AB47", 27, 46)]
        [TestCase("ABC123", 730, 122)]
        public void TranslatesAddress(string address, int expectedColumn, int expectedRow) {
            var cell = new ExcelCell(address, "");
            ClassicAssert.AreEqual(expectedColumn, cell.Column);
            ClassicAssert.AreEqual(expectedRow, cell.Row);
        }
    }
}
