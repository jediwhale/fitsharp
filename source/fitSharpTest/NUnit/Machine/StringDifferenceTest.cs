// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class StringDifferenceTest {
        [Test] public void FirstCharacterDifferent() {
            ClassicAssert.AreEqual("At 0 expected A was B", new StringDifference("A", "B").ToString());
        }

        [Test] public void SecondCharacterDifferent() {
            ClassicAssert.AreEqual("At 1 expected A was B", new StringDifference("aA", "aB").ToString());
        }

        [Test] public void LengthDifferent() {
            ClassicAssert.AreEqual("Length expected 2 was 1", new StringDifference("aA", "a").ToString());
        }

        [Test] public void UnviewableCharactersShownAsHex() {
            ClassicAssert.AreEqual("At 1 expected x20 was xfffe", new StringDifference("a ", "a\xFFFE").ToString());
        }
    }
}
