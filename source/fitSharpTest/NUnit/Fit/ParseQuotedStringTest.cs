using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fitSharp.Fit.Operators;

namespace fitSharp.Test.NUnit.Fit
{
    [TestFixture] public class ParseQuotedStringTest : ParseOperatorTest<ParseQuotedString> {
        [Test] public void CanParse() {
            Assert.IsTrue(CanParse<string>("'quoted'"), "'quoted'");
            Assert.IsTrue(CanParse<string>("\r\n'quoted with whitespace'  "), "\r\n'quoted with whitespace'  ");
            Assert.IsFalse(CanParse<string>("'half-quoted"), "'half-quoted");
            Assert.IsFalse(CanParse<string>("half-quoted'"), "half-quoted'");
            Assert.IsFalse(CanParse<string>("unquoted"), "unquoted");
            Assert.IsFalse(CanParse<int>("'1'"), "non-string");
        }

        [Test] public void ParseStripsQuotes() {
            Assert.AreEqual("content", Parse<string>("'content'"));
            Assert.AreEqual("content", Parse<string>("\r\n\t 'content'\r\n\t "));
        }
    }
}
