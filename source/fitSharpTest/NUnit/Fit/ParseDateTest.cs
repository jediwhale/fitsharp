// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseDateTest {
        ParseDate parseDate;
        DateTime result;
        string exceptionMessage;

        [SetUp] public void SetUp() {
            parseDate = new ParseDate {Processor = new CellProcessorBase()};
            exceptionMessage = string.Empty;
        }

        [Test] public void NonDateIsNotParsed() {
            Assert.IsFalse(TryParse(typeof(string), "today"));
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
                exceptionMessage = e.Message;
                return false;
            }
        }

        [Test] public void DateWithoutKeywordIsNotParsed() {
            Assert.IsFalse(TryParse(typeof(DateTime), "whenever"));
        }

        [Test] public void DateMatchesKeyword() {
            Assert.IsTrue(TryParse(typeof(DateTime), "today"));
            Assert.AreEqual(DateTime.Now.Date, result);
        }

        [Test] public void DateMatchesMixedCaseKeyword() {
            Assert.IsTrue(TryParse(typeof(DateTime), "ToDaY"));
            Assert.AreEqual(DateTime.Now.Date, result);
        }

        [Test] public void DateDoesNotMatchKeyword() {
            Assert.IsTrue(TryParse(typeof(DateTime), "today"));
            Assert.AreEqual(DateTime.Now.Date, result);
        }

        [Test] public void DaysAreAdded() {
            Assert.IsTrue(TryParse(typeof(DateTime), "today+1"));
            Assert.AreEqual(DateTime.Now.Date.AddDays(1), result);
        }

        [Test] public void DaysAreSubtracted() {
            Assert.IsTrue(TryParse(typeof(DateTime), "today-2"));
            Assert.AreEqual(DateTime.Now.Date.AddDays(-2), result);
        }

        [Test] public void SymbolIsParsed() {
            parseDate.Processor.Get<Symbols>().Save("two", 2);
            Assert.IsTrue(TryParse(typeof(DateTime), "today-<<two"));
            Assert.AreEqual(DateTime.Now.Date.AddDays(-2), result);
        }

        [Test] public void OtherModifierIsNotParsed() {
            Assert.IsFalse(TryParse(typeof(DateTime), "today:2"));
        }

        [Test] public void NonNumericModifierIsNotParsed() {
            Assert.IsFalse(TryParse(typeof(DateTime), "today+fun"));
            Assert.AreEqual("Input string was not in a correct format.", exceptionMessage);
        }
    }
}
