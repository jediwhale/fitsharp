// Copyright � 2022 Syterra Software Inc.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseMemberNameTest {
        [Test] public void ConvertsLeadingDigit() {
            ClassicAssert.AreEqual("ninestuff", Parse("9stuff").Name);
        }

        [Test]
        public void PreservesOriginalName() {
            var member = Parse("string:");
            ClassicAssert.AreEqual("string:", member.OriginalName);
        }

        static MemberName Parse(string input) {
            var parser = new ParseMemberName {Processor = Builder.CellProcessor()};
            return
                parser.Parse(typeof (ParseMemberName), TypedValue.Void, new CellTree(input)).GetValue<MemberName>();
        }
    }
}
