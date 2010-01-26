// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseByteArrayTest {

        [Test] public void ParsesHexString() {
            CheckParse("0x102031", new byte[] {16, 32, 49});
        }

        [Test] public void ParsesHexStringWithWhiteSpace() {
            CheckParse("0x 10 20 31", new byte[] {16, 32, 49});
        }

        private static void CheckParse(string input, byte[] expected) {
            var parser = new ParseByteArray { Processor =  new CellProcessorBase() };
            Assert.IsTrue(parser.CanParse(typeof (byte[]), TypedValue.Void, new StringCellLeaf(input)));
            TypedValue result = parser.Parse(typeof (byte[]), TypedValue.Void, new StringCellLeaf(input));
            Assert.AreEqual(expected, result.Value);
        }
    }
}
