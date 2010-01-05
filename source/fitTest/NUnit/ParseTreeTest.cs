// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Test.Double;
using fitlibrary.tree;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ParseTreeTest {

        [Test] public void MatchesTreeObject() {
            Parse table = new HtmlParser().Parse("<table><tr><td><ul><li>a</li><li>b</li></ul></td></tr></table>");
            var tree = new ParseTree(table.Parts.Parts.Parts);
            Assert.IsTrue(tree.Equals(new SampleTree(string.Empty, "a", "b")));
        }

        [Test] public void MatchesEmptyTreeObject() {
            var tree = new ParseTree(new Parse("ul", string.Empty, null, null));
            Assert.IsTrue(tree.Equals(new SampleTree(string.Empty)));
        }

    }
}
