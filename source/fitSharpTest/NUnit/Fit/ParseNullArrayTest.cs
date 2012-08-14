// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;
using fitSharp.Fit.Operators;

namespace fitSharp.Test.NUnit.Fit
{
    [TestFixture] public class ParseNullArrayTest : ParseOperatorTest<ParseNullArray> {
        [Test] public void CanParse() {
            Assert.IsTrue(CanParse<object[]>("null"), "null");
            Assert.IsTrue(CanParse<object[]>("\r\n null \r\n\t"), "null with whitespace");
            Assert.IsFalse(CanParse<object[]>("not null"), "not null");
            Assert.IsFalse(CanParse<string>("null"), "non-array");
        }

        [Test] public void ParseAlwaysReturnsNull() {
            Assert.IsNull(Parse<object[]>("null"), "null");
            Assert.IsNull(Parse<object[]>("not null"), "not null");
            Assert.IsNull(Parse<string>("not null"), "non-array and not null");
        }
    }
}
