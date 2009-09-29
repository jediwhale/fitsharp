using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ParseDateTest {
        private ParseDate parseDate;
        private DateTime result;
        private string exceptionMessage;

        [SetUp] public void SetUp() {
            parseDate = new ParseDate {Processor = new CellProcessorBase()};
            exceptionMessage = string.Empty;
        }

        [Test] public void NonDateIsNotParsed() {
            Assert.IsFalse(TryParse(typeof(string), "today"));
        }

        private bool TryParse(Type type, string expected) {
            try {
                if (parseDate.CanParse(type, TypedValue.Void, new StringCellLeaf(expected))) {
                    TypedValue returnValue = parseDate.Parse(type, TypedValue.Void, new StringCellLeaf(expected));
                    result = returnValue.GetValue<DateTime>();
                    return true;
                }
                return false;
            }
            catch (Exception e) {
                exceptionMessage = e.InnerException.Message;
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
            var symbol = new Symbol("two", 2);
            parseDate.Processor.Store(symbol);
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
