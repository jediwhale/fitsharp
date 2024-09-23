// Copyright � 2017 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Test.Double;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseBooleanTest {
        private ParseBoolean parseBoolean;

        [SetUp] public void SetUp() {
            parseBoolean = new ParseBoolean {Processor = Builder.CellProcessor()};
        }

        [Test]
        public void ParseValidStrings() {
            var validTrueStrings = new [] {
                "Y", "y", "\n\ty\n",
                "YES", "yes", "YeS", "yEs", "\n\tyes\n",
                "true", "TRUE", "TrUe", "tRuE", "\n\ttrue\n"
            };
            var validFalseStrings = new [] {
                "N", "n", "\n\tn\n",
                "NO", "no", "No", "nO", "\n\tno\n",
                "false", "FALSE", "fAlSe", "FaLsE", "\n\tfalse\n"
            };
            foreach (string validString in validTrueStrings) {
                ClassicAssert.IsTrue(Parse(validString), "Parsing '" + validString + "'");
            }
            foreach (string validString in validFalseStrings) {
                ClassicAssert.IsFalse(Parse(validString), "Parsing '" + validString + "'");
            }
        }

        [Test] public void ParseInvalidString() {
            ClassicAssert.Throws<FormatException>(() => Parse("garbage"));
        }

        private bool Parse(string validString) {
            if (!parseBoolean.CanParse(typeof (bool), TypedValue.Void, TestUtils.CreateCell(validString)))
                return false;

            TypedValue result = parseBoolean.Parse(typeof (bool), TypedValue.Void, TestUtils.CreateCell(validString));
            return result.GetValue<bool>();
        }
    }
}