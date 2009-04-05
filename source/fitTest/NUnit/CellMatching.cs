// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fit.Engine;
using fitlibrary;
using fitlibrary.tree;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class CellMatching: Fixture {

        [Test] public void NullEqualsNullCell() {
            Assert.IsTrue(IsEqual(new Parse("td", null, null, null), null));
        }

        private static bool IsEqual(Parse cell, object value) {
            return new CellOperation(new Service.Service()).Compare(new TypedValue(value), cell);
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
            Parse cell = new Parse("td", "something", null, null);
            var fixture = new Fixture {Service = new Service.Service()};
            fixture.CellOperation.Check(fixture, new TypedValue("something"), cell);
            Assert.AreEqual("\n<td class=\"pass\">something</td>", cell.ToString());
        }
    
        [Test] public void MarksSameArrayCellAsRight() {
            Parse cell = new Parse("td", "something,more", null, null);
            var fixture = new Fixture {Service = new Service.Service()};
            fixture.CellOperation.Check(fixture, new TypedValue(new string[] {"something", "more"}), cell);
            Assert.AreEqual("\n<td class=\"pass\">something,more</td>", cell.ToString());
        }
    
        [Test] public void StringDoesntEqualDifferentStringCell() {
            Assert.IsFalse(IsEqual(new Parse("td", "something else", null, null), "something"));
        }
    
        [Test] public void MarksDifferentStringCellAsWrong() {
            Parse cell = new Parse("td", "something else", null, null);
            var fixture = new Fixture {Service = new Service.Service()};
            fixture.CellOperation.Check(fixture, new TypedValue("something"), cell);
            Assert.AreEqual("\n<td class=\"fail\">something else <span class=\"fit_label\">expected</span><hr />something <span class=\"fit_label\">actual</span></td>", cell.ToString());
        }
    
        [Test] public void TreeEqualsSameTreeCell() {
            object actual = new ListTree(string.Empty, new ListTree[]{new ListTree("a")});
            Parse table = HtmlParser.Instance.Parse("<table><tr><td><ul><li>a</li></ul></td></tr></table>");
            Assert.IsTrue(IsEqual(table.Parts.Parts, actual));
        }
    
        [Test] public void ListEqualsSameTableCell() {
            ArrayList actual = new ArrayList();
            actual.Add(new Name("joe", "smith"));
            Parse table = HtmlParser.Instance.Parse("<table><tr><td><table><tr><td>first</td><td>last</td></tr><tr><td>joe</td><td>smith</td></tr></table></td></tr></table>");
            Assert.IsTrue(IsEqual(table.Parts.Parts, actual));
        }

        [Test] public void MarksSameTableCellAsRight() {
            ArrayList actual = new ArrayList();
            actual.Add(new Name("joe", "smith"));
            Parse table = HtmlParser.Instance.Parse("<table><tr><td><table><tr><td>first</td><td>last</td></tr><tr><td>joe</td><td>smith</td></tr></table></td></tr></table>");
            var fixture = new Fixture {Service = new Service.Service()};
            fixture.CellOperation.Check(fixture, new TypedValue(actual), table.Parts.Parts);
            Assert.AreEqual("<td><table><tr><td>first</td><td>last</td></tr><tr><td class=\"pass\">joe</td><td class=\"pass\">smith</td></tr></table></td>", table.Parts.Parts.ToString());
        }

        [Test] public void MarksExtraTableHeaderAsError() {
            ArrayList actual = new ArrayList();
            actual.Add(new Name("joe", "smith"));
            Parse table = HtmlParser.Instance.Parse("<table><tr><td><table><tr><td>first</td><td>last</td><td>address</td></tr><tr><td>joe</td><td>smith</td><td></td></tr></table></td></tr></table>");
            var fixture = new Fixture {Service = new Service.Service()};
            fixture.CellOperation.Check(fixture, new TypedValue(actual), table.Parts.Parts);
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