// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fit.Model;
using fitlibrary;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ValueArrayTest {

        [Test] public void SingleRowReturnsEachCell() {
            Parse rows = new HtmlParser().Parse("<table><tr><td>aaa</td><td>bb</td><td>c</td></tr></table>");
            ArrayList results = ProcessRows(rows.Parts);
            Assert.AreEqual(3, results.Count,  "count");
            Assert.AreEqual("aaa", results[0], "first");
            Assert.AreEqual("bb", results[1], "second");
            Assert.AreEqual("c", results[2], "third");
        }
    
        [Test] public void MultipleRowsReturnsEachCell() {
            Parse rows = new HtmlParser().Parse("<table><tr><td>xxx</td></tr><tr><td>aaa</td><td>bb</td><td>c</td></tr></table>");
            ArrayList results = ProcessRows(rows.Parts);
            Assert.AreEqual(3, results.Count,  "count");
            Assert.AreEqual("aaa", results[0], "first");
            Assert.AreEqual("bb", results[1], "second");
            Assert.AreEqual("c", results[2], "third");
        }
    
        [Test] public void RepeatKeywordReturnsPreviousValue() {
            Parse rows = new HtmlParser().Parse("<table><tr><td>xxx</td><td>yy</td></tr><tr><td>aaa</td><td>ditto</td><td>c</td></tr></table>");
            ArrayList results = ProcessRows(rows.Parts);
            Assert.AreEqual(3, results.Count,  "count");
            Assert.AreEqual("aaa", results[0], "first");
            Assert.AreEqual("yy", results[1], "second");
            Assert.AreEqual("c", results[2], "third");
        }

        [Test] public void RepeatKeywordWithNoPreviousValue() {
            Parse rows = new HtmlParser().Parse("<table><tr><td>xxx</td><td>yy</td></tr><tr><td>aaa</td><td>bb</td><td>ditto</td></tr></table>");
            ArrayList results = ProcessRows(rows.Parts);
            Assert.AreEqual(3, results.Count,  "count");
            Assert.AreEqual("aaa", results[0], "first");
            Assert.AreEqual("bb", results[1], "second");
            Assert.AreEqual("ditto", results[2], "third");
        }
    
        private static ArrayList ProcessRows(Parse theRows) {
            var values = new ValueArray("ditto");
            ArrayList results = null;
            foreach (Parse row in new CellRange(theRows).Cells) {
                results = new ArrayList();
                //values.LoadSource(new CellRange(row.Parts).Cells);
                foreach (Parse cell in values.GetCells(new CellRange(row.Parts).Cells).Cells) {
                    results.Add(cell.Text);
                }
            }
            return results;
        }


    }
}