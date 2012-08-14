using System;
using NUnit.Framework;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using fitSharp.Fit.Operators;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class ParseNullTest {
        ParseNull parser;

        [SetUp] public void SetUp() {
            parser = new ParseNull { Processor = Builder.CellProcessor() };
        }

        [Test] public void CanParse() {
            Assert.IsTrue(CanParse("null"));
            Assert.IsTrue(CanParse("NULL"));
            Assert.IsTrue(CanParse("\r\n null \r\n\t"));
            Assert.IsFalse(CanParse("not null"));
        }

        [Test] public void ParseAlwaysReturnsNull() {
            Assert.IsNull(Parse("null"));
            Assert.IsNull(Parse("bob"));
        }


        bool CanParse(string cellContent) {
            return parser.CanParse(typeof(string), TypedValue.Void, new CellTreeLeaf(cellContent));
        }

        string Parse(string cellContent) {
            TypedValue result = parser.Parse(typeof(string), TypedValue.Void, new CellTreeLeaf(cellContent));
            return result.GetValueAs<string>();
        }
    }
}
