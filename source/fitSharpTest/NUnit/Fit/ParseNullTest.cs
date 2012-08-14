using System;
using NUnit.Framework;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;

namespace fitSharp.Test.NUnit.Fit
{
    [TestFixture] public class ParseNullTest : ParseOperatorTest<ParseNull> {
        [Test] public void CanParse() {
            Assert.IsTrue(CanParse<string>("null"), "null");
            Assert.IsTrue(CanParse<string>("NULL"), "NULL");
            Assert.IsTrue(CanParse<string>("\r\n null \r\n\t"), "null with whitespace");
            Assert.IsFalse(CanParse<string>("not null"), "not null");
        }

        [Test] public void ParseAlwaysReturnsNull() {
            Assert.IsNull(Parse<string>("null"), "null");
            Assert.IsNull(Parse<string>("not null"), "not null");
        }
    }
}
