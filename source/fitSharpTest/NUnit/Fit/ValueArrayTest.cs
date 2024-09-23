// Copyright � 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ValueArrayTest {

        [Test] public void SingleRowReturnsEachCell() {
            var rows = new CellTree(new CellTree("aaa", "bb", "c"));
            var results = ProcessRows(rows.Branches);
            ClassicAssert.AreEqual(3, results.Count,  "count");
            ClassicAssert.AreEqual("aaa", results[0], "first");
            ClassicAssert.AreEqual("bb", results[1], "second");
            ClassicAssert.AreEqual("c", results[2], "third");
        }
    
        [Test] public void MultipleRowsReturnsEachCell() {
            var rows = new CellTree(new CellTree("xxx"), new CellTree("aaa", "bb", "c"));
            var results = ProcessRows(rows.Branches);
            ClassicAssert.AreEqual(3, results.Count,  "count");
            ClassicAssert.AreEqual("aaa", results[0], "first");
            ClassicAssert.AreEqual("bb", results[1], "second");
            ClassicAssert.AreEqual("c", results[2], "third");
        }
    
        [Test] public void RepeatKeywordReturnsPreviousValue() {
            var rows = new CellTree(new CellTree("xxx", "yy"), new CellTree("aaa", "ditto", "c"));
            var results = ProcessRows(rows.Branches);
            ClassicAssert.AreEqual(3, results.Count,  "count");
            ClassicAssert.AreEqual("aaa", results[0], "first");
            ClassicAssert.AreEqual("yy", results[1], "second");
            ClassicAssert.AreEqual("c", results[2], "third");
        }

        [Test] public void RepeatKeywordWithNoPreviousValue() {
            var rows = new CellTree(new CellTree("xxx", "yy"), new CellTree("aaa", "bb", "ditto"));
            var results = ProcessRows(rows.Branches);
            ClassicAssert.AreEqual(3, results.Count,  "count");
            ClassicAssert.AreEqual("aaa", results[0], "first");
            ClassicAssert.AreEqual("bb", results[1], "second");
            ClassicAssert.AreEqual("ditto", results[2], "third");
        }
    
        static IList<string> ProcessRows(IEnumerable<Tree<Cell>> theRows) {
            var values = new ValueArray("ditto");
            List<string> results = null;
            foreach (var row in theRows) {
                results = values.GetCells(row.Branches).Branches.Select(cell => cell.Value.Text).ToList();
            }
            return results;
        }
    }
}
