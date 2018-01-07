// Copyright © 2018 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ComposeStoryTestHtmlTest {

        [Test] public void HtmlStringIsParsed() {
            var service = new Service.Service();
            Tree<Cell> result = service.Compose(StoryTestSource.FromString("<table><tr><td>hello</td></tr></table>"));
            var table = ((Parse)result).Parts;
            Assert.AreEqual("<table>", table.Tag);
            Parse cell = table.Parts.Parts;
            Assert.AreEqual("<td>", cell.Tag);
            Assert.AreEqual("hello", cell.Body);
        }

        [Test] public void NoTablesReturnsEmptyTree() {
            var service = new Service.Service();
            Tree<Cell> result = service.Compose(StoryTestSource.FromString("<b>stuff</b>"));
            Assert.AreEqual(0, result.Branches.Count);
        }

        [Test] public void SimpleHtmlStringIsGenerated() {
            CheckRoundTrip("<table><tr><td>hello</td></tr></table>");
        }

        [Test] public void ComplexHtmlStringIsGenerated() {
            CheckRoundTrip("some stuff <table border=\"1\"><tr> umm <td>hello</td></tr></table> and more");
        }

        [Test] public void MultipleTablesAreGenerated() {
            CheckRoundTrip("some stuff <table border=\"1\"><tr> umm <td>hello</td></tr></table> and more <table><tr><td>more</td></tr></table>");
        }

        private static void CheckRoundTrip(string input) {
            var service = new Service.Service();
            var source = service.Compose(StoryTestSource.FromString(input));
            var result = source.WriteBranches();
            Assert.AreEqual(input, result);
        }
    }
}
