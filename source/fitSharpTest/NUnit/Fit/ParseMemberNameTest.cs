// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseMemberNameTest {
        [Test] public void ConvertsLeadingDigit() {
            Assert.AreEqual("ninestuff", Parse("9stuff").Name);
        }

        [Test] public void ParsesGenericType() {
            var member = Parse("generic of System.String");
            Assert.AreEqual("genericofsystemstring", member.Name);
            Assert.True(member.Matches(typeof(ParseMemberSample).GetMethod("Generic")));
        }

        [Test]
        public void PreservesOriginalName() {
            var member = Parse("string:");
            Assert.AreEqual("string:", member.OriginalName);
        }

        [Test]
        public void ParsesExtensionMethod() {
            var member = Parse("extension(fitSharp.Test.NUnit.Fit.ParseMemberExtension)");
            Assert.True(member.Matches(typeof(ParseMemberExtension).GetMethod("Extension")));
        }

        static MemberName Parse(string input) {
            var processor = Builder.CellProcessor();
            processor.ApplicationUnderTest.AddAssembly(typeof(ParseMemberNameTest).Assembly.Location);
            var parser = new ParseMemberName {Processor = processor};
            return
                parser.Parse(typeof (ParseMemberName), TypedValue.Void, new CellTree(input)).GetValue<MemberName>();
        }
    }

    public class ParseMemberSample {
        public bool Generic<T>() { return true; }
    }

    public static class ParseMemberExtension {
        public static void Extension(this ParseMemberSample sample) {}
    }
}