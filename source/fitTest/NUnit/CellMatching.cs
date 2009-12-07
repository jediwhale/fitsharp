// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitSharp.Fit.Model;
using fitlibrary.tree;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class CellMatching: Fixture {

        [Test] public void NullEqualsNullCell() {
            Assert.IsTrue(IsEqual(new Parse("td", null, null, null), null));
        }

        private static bool IsEqual(Tree<Cell> cell, object value) {
            return new CellOperationImpl(new Service.Service()).Compare(new TypedValue(value), cell);
        }

        [Test] public void NullEqualsEmptyCell() {
            Assert.IsTrue(IsEqual(new Parse("td", string.Empty, null, null), null));
        }
    
        [Test] public void NullDoesntEqualFullCell() {
            Assert.IsFalse(IsEqual(new Parse("td", "something", null, null), null));
        }

        [Test] public void StringEqualsSameStringCell() {
            Assert.IsTrue(IsEqual(new Parse("td", "something", null, null), "something"));
        }
    
        [Test] public void MarksSameStringCellAsRight() {
            var cell = new Parse("td", "something", null, null);
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.CellOperation.Check(null, new TypedValue("something"), cell);
            Assert.AreEqual("\n<td class=\"pass\">something</td>", cell.ToString());
        }
    
        [Test] public void MarksSameArrayCellAsRight() {
            var cell = new Parse("td", "something,more", null, null);
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.CellOperation.Check(null, new TypedValue(new [] {"something", "more"}), cell);
            Assert.AreEqual("\n<td class=\"pass\">something,more</td>", cell.ToString());
        }
    
        [Test] public void StringDoesntEqualDifferentStringCell() {
            Assert.IsFalse(IsEqual(new Parse("td", "something else", null, null), "something"));
        }
    
        [Test] public void MarksDifferentStringCellAsWrong() {
            var cell = new Parse("td", "something else", null, null);
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.CellOperation.Check(null, new TypedValue("something"), cell);
            Assert.AreEqual("\n<td class=\"fail\">something else <span class=\"fit_label\">expected</span><hr />something <span class=\"fit_label\">actual</span></td>", cell.ToString());
        }
    
        [Test] public void TreeEqualsSameTreeCell() {
            object actual = new ListTree(string.Empty, new []{new ListTree("a")});
            Parse table = new HtmlParser().Parse("<table><tr><td><ul><li>a</li></ul></td></tr></table>");
            Assert.IsTrue(IsEqual(table.Parts.Parts, actual));
        }
    
        [Test] public void ListEqualsSameTableCell() {
            var actual = new ArrayList {new Name("joe", "smith")};
            Parse table = new HtmlParser().Parse("<table><tr><td><table><tr><td>first</td><td>last</td></tr><tr><td>joe</td><td>smith</td></tr></table></td></tr></table>");
            Assert.IsTrue(IsEqual(table.Parts.Parts, actual));
        }

        [Test] public void MarksSameTableCellAsRight() {
            var actual = new ArrayList {new Name("joe", "smith")};
            Parse table = new HtmlParser().Parse("<table><tr><td><table><tr><td>first</td><td>last</td></tr><tr><td>joe</td><td>smith</td></tr></table></td></tr></table>");
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.CellOperation.Check(null, new TypedValue(actual), table.Parts.Parts);
            Assert.AreEqual("<td><table><tr><td>first</td><td>last</td></tr><tr><td class=\"pass\">joe</td><td class=\"pass\">smith</td></tr></table></td>", table.Parts.Parts.ToString());
        }

        [Test] public void MarksExtraTableHeaderAsError() {
            var actual = new ArrayList {new Name("joe", "smith")};
            Parse table = new HtmlParser().Parse("<table><tr><td><table><tr><td>first</td><td>last</td><td>address</td></tr><tr><td>joe</td><td>smith</td><td></td></tr></table></td></tr></table>");
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.CellOperation.Check(null, new TypedValue(actual), table.Parts.Parts);
            Assert.AreEqual("<td><table><tr><td>first</td><td>last</td><td class=\"error\">address<hr /><pre><div class=\"fit_stacktrace\">fitlibrary.exception.FitFailureException: Column 'address' not used.</div></pre></td></tr><tr><td class=\"pass\">joe</td><td class=\"pass\">smith</td><td></td></tr></table></td>", table.Parts.Parts.ToString());
        }

        private class Name {
            public string First;
            public string Last;
            public Name(string theFirst, string theLast) {
                First = theFirst;
                Last = theLast;
            }
        }
    }
}