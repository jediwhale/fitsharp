// Copyright � 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitlibrary.tree;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;
using TestStatus=fitSharp.Fit.Model.TestStatus;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture] public class CellMatching {

        [Test] public void NullEqualsNullCell() {
            ClassicAssert.IsTrue(IsEqual(new Parse("td", null, null, null), null));
        }

        private static bool IsEqual(Tree<Cell> cell, object value) {
            return new Service.Service().Compare(new TypedValue(value), cell);
        }

        [Test] public void NullEqualsEmptyCell() {
            ClassicAssert.IsTrue(IsEqual(new Parse("td", string.Empty, null, null), null));
        }
    
        [Test] public void NullDoesntEqualFullCell() {
            ClassicAssert.IsFalse(IsEqual(new Parse("td", "something", null, null), null));
        }

        [Test] public void StringEqualsSameStringCell() {
            ClassicAssert.IsTrue(IsEqual(new Parse("td", "something", null, null), "something"));
        }
    
        [Test] public void MarksSameStringCellAsRight() {
            var cell = new Parse("td", "something", null, null);
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.Processor.Check(new TypedValue("something"), cell);
            ClassicAssert.AreEqual("\n<td class=\"pass\">something</td>", cell.ToString());
        }
    
        [Test] public void MarksSameArrayCellAsRight() {
            var cell = new Parse("td", "something,more", null, null);
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.Processor.Check(new TypedValue(new [] {"something", "more"}), cell);
            ClassicAssert.AreEqual("\n<td class=\"pass\">something,more</td>", cell.ToString());
        }
    
        [Test] public void StringDoesntEqualDifferentStringCell() {
            ClassicAssert.IsFalse(IsEqual(new Parse("td", "something else", null, null), "something"));
        }
    
        [Test] public void MarksDifferentStringCellAsWrong() {
            var cell = new Parse("td", "something else", null, null);
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.Processor.Check(new TypedValue("something"), cell);
            ClassicAssert.AreEqual(fitSharp.Fit.Model.TestStatus.Wrong, cell.GetAttribute(CellAttribute.Status));
            ClassicAssert.AreEqual("something", cell.GetAttribute(CellAttribute.Actual));
            ClassicAssert.IsTrue(cell.HasAttribute(CellAttribute.Difference));
        }
    
        [Test] public void TreeEqualsSameTreeCell() {
            object actual = new ListTree(string.Empty, new []{new ListTree("a")});
            Parse table = Parse.ParseFrom("<table><tr><td><ul><li>a</li></ul></td></tr></table>");
            ClassicAssert.IsTrue(IsEqual(table.Parts.Parts, actual));
        }
    
        [Test] public void ListEqualsSameTableCell() {
            var actual = new ArrayList {new Name("joe", "smith")};
            Parse table = Parse.ParseFrom("<table><tr><td><table><tr><td>first</td><td>last</td></tr><tr><td>joe</td><td>smith</td></tr></table></td></tr></table>");
            ClassicAssert.IsTrue(IsEqual(table.Parts.Parts, actual));
        }

        [Test] public void MarksSameTableCellAsRight() {
            var actual = new ArrayList {new Name("joe", "smith")};
            Parse table = Parse.ParseFrom("<table><tr><td><table><tr><td>first</td><td>last</td></tr><tr><td>joe</td><td>smith</td></tr></table></td></tr></table>");
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.Processor.Check(new TypedValue(actual), table.Parts.Parts);
            var passRow = table.Parts.Parts.Parts.Parts.More;
            ClassicAssert.AreEqual(TestStatus.Right, passRow.Parts.Value.GetAttribute(CellAttribute.Status));
            ClassicAssert.AreEqual(TestStatus.Right, passRow.Parts.More.Value.GetAttribute(CellAttribute.Status));
        }

        [Test] public void MarksExtraTableHeaderAsError() {
            var actual = new ArrayList {new Name("joe", "smith")};
            Parse table = Parse.ParseFrom("<table><tr><td><table><tr><td>first</td><td>last</td><td>address</td></tr><tr><td>joe</td><td>smith</td><td></td></tr></table></td></tr></table>");
            var fixture = new Fixture {Processor = new Service.Service()};
            fixture.Processor.Check(new TypedValue(actual), table.Parts.Parts);
            var errorCell = table.Parts.Parts.Parts.Parts.Parts.More.More.Value;
            ClassicAssert.IsTrue(errorCell.HasAttribute(CellAttribute.Exception));
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