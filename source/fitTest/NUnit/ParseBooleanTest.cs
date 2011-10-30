// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using fitSharp.Test.Double.Fit;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseBooleanTest {
        private ParseBoolean parseBoolean;

        [SetUp] public void SetUp() {
            parseBoolean = new ParseBoolean {Processor = Builder.CellProcessor()};
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
            Assert.IsTrue(parseBoolean.CanParse(typeof (bool), TypedValue.Void, TestUtils.CreateCell(validString)));
            TypedValue result = parseBoolean.Parse(typeof (bool), TypedValue.Void, TestUtils.CreateCell(validString));
            return result.GetValue<bool>();
        }
    }
}