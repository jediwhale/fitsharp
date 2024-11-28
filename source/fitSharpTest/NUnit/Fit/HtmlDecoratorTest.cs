// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Runner;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class HtmlDecoratorTest {

        [Test]
        public void AddsTextToStart() {
            ClassicAssert.AreEqual("newtext" + Environment.NewLine + "existingtext", HtmlDecorator.AddToStart("newtext", "existingtext"));
        }

        [Test]
        public void AddsTextAfterDocType() {
            ClassicAssert.AreEqual("<!doctype stuff>" + Environment.NewLine + "newtextexistingtext", HtmlDecorator.AddToStart("newtext", "<!doctype stuff>existingtext"));
        }
         
    }
}
