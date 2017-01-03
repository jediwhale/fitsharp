// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Runner;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class HtmlDecoratorTest {

        [Test]
        public void AddsTextToStart() {
            Assert.AreEqual("newtext\r\nexistingtext", HtmlDecorator.AddToStart("newtext", "existingtext"));
        }

        [Test]
        public void AddsTextAfterDocType() {
            Assert.AreEqual("<!doctype stuff>\r\nnewtextexistingtext", HtmlDecorator.AddToStart("newtext", "<!doctype stuff>existingtext"));
        }
         
    }
}
