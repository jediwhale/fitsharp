// Copyright � 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;
using fitSharp.Fit.Operators;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit 

{
    [TestFixture] public class ParseNullTest : ParseOperatorTest<ParseNull> {
        [Test] public void CanParse() {
            ClassicAssert.IsTrue(CanParse<string>("null"), "null");
            ClassicAssert.IsTrue(CanParse<string>("NULL"), "NULL");
            ClassicAssert.IsTrue(CanParse<string>("\r\n null \r\n\t"), "null with whitespace");
            ClassicAssert.IsFalse(CanParse<string>("not null"), "not null");
        }

        [Test] public void ParseAlwaysReturnsNull() {
            ClassicAssert.IsNull(Parse<string>("null"), "null");
            ClassicAssert.IsNull(Parse<string>("not null"), "not null");
        }
    }
}
