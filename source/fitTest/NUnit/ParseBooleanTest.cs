using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseBooleanTest {
        private ParseBoolean parseBoolean;
        private Processor<Cell> processor;

        [SetUp] public void SetUp() {
            parseBoolean = new ParseBoolean();
            processor = new Processor<Cell>();
        }

        [Test]
        public void ParseValidStrings() {
            var validTrueStrings = new [] {
                "Y", "y",
                "YES", "yes", "YeS", "yEs",
                "true", "TRUE", "TrUe", "tRuE"
            };
            var validFalseStrings = new [] {
                "N", "n",
                "NO", "no", "No", "nO",
                "false", "FALSE", "fAlSe", "FaLsE"
            };
            foreach (string validString in validTrueStrings) {
                Assert.IsTrue(Parse(validString));
            }
            foreach (string validString in validFalseStrings) {
                Assert.IsFalse(Parse(validString));
            }
        }

        [Test] public void ParseInvalidString() {
            try {
                Parse("garbage");
            }
            catch (Exception e) {
                Assert.IsTrue(e is FormatException);
                return;
            }
            Assert.Fail();
        }

        private bool Parse(string validString) {
            TypedValue result = TypedValue.Void;
            Assert.IsTrue(parseBoolean.TryParse(processor, typeof (bool), TypedValue.Void, TestUtils.CreateCell(validString), ref result));
            return (bool)result.Value;
        }
    }
}