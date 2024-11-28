// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseDateTest {
        ParseDate parseDate;
        DateTime result;
        string exceptionType;

        [SetUp] public void SetUp() {
            parseDate = new ParseDate {Processor = Builder.CellProcessor()};
            exceptionType = string.Empty;
        }

        [Test] public void NonDateIsNotParsed() {
            ClassicAssert.IsFalse(TryParse(typeof(string), "today"));
        }

        bool TryParse(Type type, string expected) {
            try {
                if (parseDate.CanParse(type, TypedValue.Void, new CellTreeLeaf(expected))) {
                    TypedValue returnValue = parseDate.Parse(type, TypedValue.Void, new CellTreeLeaf(expected));
                    result = returnValue.GetValue<DateTime>();
                    return true;
                }
                return false;
            }
            catch (Exception e) {
                exceptionType = (e.InnerException ?? e).GetType().ToString();
                return false;
            }
        }

        [Test] public void DateWithoutKeywordIsNotParsed() {
            ClassicAssert.IsFalse(TryParse(typeof(DateTime), "whenever"));
        }

        [Test] public void DateMatchesKeyword() {
            ClassicAssert.IsTrue(TryParse(typeof(DateTime), "today"));
            ClassicAssert.AreEqual(DateTime.Now.Date, result);
        }

        [Test] public void DateMatchesMixedCaseKeyword() {
            ClassicAssert.IsTrue(TryParse(typeof(DateTime), "ToDaY"));
            ClassicAssert.AreEqual(DateTime.Now.Date, result);
        }

        [Test] public void DateMatchesKeywordWithWhitespace() {
            ClassicAssert.IsTrue(TryParse(typeof(DateTime), "\r\n\t today\r\n\t "));
            ClassicAssert.AreEqual(DateTime.Now.Date, result);
        }

        [Test] public void DaysAreAdded() {
            ClassicAssert.IsTrue(TryParse(typeof(DateTime), "today+1"));
            ClassicAssert.AreEqual(DateTime.Now.Date.AddDays(1), result);
        }

        [Test] public void DaysAreSubtracted() {
            ClassicAssert.IsTrue(TryParse(typeof(DateTime), "today-2"));
            ClassicAssert.AreEqual(DateTime.Now.Date.AddDays(-2), result);
        }

        [Test] public void SymbolIsParsed() {
            parseDate.Processor.Get<Symbols>().Save("two", 2);
            ClassicAssert.IsTrue(TryParse(typeof(DateTime), "today-<<two"));
            ClassicAssert.AreEqual(DateTime.Now.Date.AddDays(-2), result);
        }

        [Test] public void OtherModifierIsNotParsed() {
            ClassicAssert.IsFalse(TryParse(typeof(DateTime), "today:2"));
        }

        [Test] public void EmptyModifierIsNotParsed() {
            ClassicAssert.IsFalse(TryParse(typeof(DateTime), "today+"));
            ClassicAssert.IsFalse(TryParse(typeof(DateTime), "today-"));
        }

        [Test] public void NonNumericModifierIsNotParsed() {
            ClassicAssert.IsFalse(TryParse(typeof(DateTime), "today+fun"));
            ClassicAssert.AreEqual("System.FormatException", exceptionType);
        }
    }
}
