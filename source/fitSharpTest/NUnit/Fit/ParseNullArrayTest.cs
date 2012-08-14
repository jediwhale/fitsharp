using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fitSharp.Fit.Operators;

namespace fitSharp.Test.NUnit.Fit {
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
