// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class IdentifierNameTest {
        [Test] public void MatchesString() {
            Assert.IsTrue(new IdentifierName("stuff").Matches("stuff"));
        }

        [Test] public void MatchIgnoresCase() {
            Assert.IsTrue(new IdentifierName("stUFf").Matches("Stuff"));
        }

        [Test] public void WhitespaceIsTrimmed() {
            Assert.IsTrue(new IdentifierName(" stuff\n").Matches("stuff"));
        }

        [Test] public void NoInputIsEmpty() {
            Assert.IsTrue(new IdentifierName(" \n").IsEmpty);
        }

        [Test] public void StartsString() {
            Assert.IsTrue(new IdentifierName("hi").IsStartOf("Hi Mom!"));
        }
    }
}