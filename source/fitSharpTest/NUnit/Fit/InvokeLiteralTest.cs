// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class InvokeLiteralTest {

        [Test]
        public void ProcessesOneParameter() {
            AssertCanInvoke(true, new CellTree("astring"));
        }

        [Test]
        public void DoesNotProcessTwoParameters() {
            AssertCanInvoke(false, new CellTree("astring", "bstring"));
        }

        [Test]
        public void DoesNotProcessZeroParameters() {
            AssertCanInvoke(false, new CellTree());
        }

        [Test]
        public void ParsesBoolean() {
            AssertParses(true, "boolean:", "yes");
        }

        [Test]
        public void ParsesByte() {
            AssertParses((byte)12, "byte:", "12");
        }

        [Test]
        public void ParsesCharacter() {
            AssertParses('a', "character:", "a");
        }

        [Test]
        public void ParsesDecimal() {
            AssertParses(1.23M, "decimal:", "1.23");
        }

        [Test]
        public void ParsesDouble() {
            AssertParses(1.23D, "double:", "1.23");
        }

        [Test]
        public void ParsesFloat() {
            AssertParses(1.23F, "float:", "1.23");
        }

        [Test]
        public void ParsesInteger() {
            AssertParses(123, "integer:", "123");
        }

        [Test]
        public void ParsesLong() {
            AssertParses(123L, "long:", "123");
        }

        [Test]
        public void ParsesShort() {
            AssertParses((short)123, "short:", "123");
        }

        [Test]
        public void ParsesString() {
            AssertParses("astring", "string:", "astring");
        }

        static void AssertCanInvoke(bool expected, Tree<Cell> parameters) {
            var literal = new InvokeLiteral {Processor = Builder.CellProcessor()};
            Assert.AreEqual(expected, literal.CanInvoke(TypedValue.Void, new MemberName("string:"), parameters));
        }

        static void AssertParses<T>(T expected, string keyword, string parameter) {
            var literal = new InvokeLiteral {Processor = Builder.CellProcessor()};
            var result = literal.Invoke(TypedValue.Void, new MemberName(keyword), new CellTree(parameter));
            Assert.AreEqual(typeof(T), result.Type);
            Assert.AreEqual(expected, result.GetValue<T>());
        }
    }
}
