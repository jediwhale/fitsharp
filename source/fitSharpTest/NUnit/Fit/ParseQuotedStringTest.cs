﻿// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;
using fitSharp.Fit.Operators;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit
{
    [TestFixture] public class ParseQuotedStringTest : ParseOperatorTest<ParseQuotedString> {
        [Test] public void CanParse() {
            ClassicAssert.IsTrue(CanParse<string>("'quoted'"), "'quoted'");
            ClassicAssert.IsTrue(CanParse<string>("\r\n'quoted with whitespace'  "), "\r\n'quoted with whitespace'  ");
            ClassicAssert.IsFalse(CanParse<string>("'half-quoted"), "'half-quoted");
            ClassicAssert.IsFalse(CanParse<string>("half-quoted'"), "half-quoted'");
            ClassicAssert.IsFalse(CanParse<string>("unquoted"), "unquoted");
            ClassicAssert.IsFalse(CanParse<int>("'1'"), "non-string");
        }

        [Test] public void ParseStripsQuotes() {
            ClassicAssert.AreEqual("content", Parse<string>("'content'"));
            ClassicAssert.AreEqual("content", Parse<string>("\r\n\t 'content'\r\n\t "));
        }
    }
}
