// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class IdentifierNameTest {
        [Test] public void MatchesString() {
            ClassicAssert.IsTrue(new IdentifierName("stuff").Matches("stuff"));
        }

        [Test] public void MatchIgnoresCase() {
            ClassicAssert.IsTrue(new IdentifierName("stUFf").Matches("Stuff"));
        }

        [Test] public void WhitespaceIsTrimmed() {
            ClassicAssert.IsTrue(new IdentifierName(" stuff\n").Matches("stuff"));
        }

        [Test] public void UnderscoreInMatchIsIgnored() {
            ClassicAssert.IsTrue(new IdentifierName("stuff").Matches("stu_ff"));
        }

        [Test] public void UnderscoreInNameIsIgnored() {
            ClassicAssert.IsTrue(new IdentifierName("stu_ff").Matches("stu_ff"));
        }

        [Test] public void NoInputIsEmpty() {
            ClassicAssert.IsTrue(new IdentifierName(" \n").IsEmpty);
        }

        [Test] public void StartsString() {
            ClassicAssert.IsTrue(new IdentifierName("hi").IsStartOf("Hi Mom!"));
        }
    }
}