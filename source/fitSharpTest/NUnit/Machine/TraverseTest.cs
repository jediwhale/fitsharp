// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class TraverseTest {
        [SetUp] public void SetUp() {
            nodesVisited = new List<string>();
        }

        [Test] public void VisitsFirstRow() {
            new Traverse<Cell>()
                .Rows.Header(Visit)
                .VisitTable(new CellTree("fixture", "row1", "row2"));
            Assert.AreEqual("row1", nodesVisited.Join(","));
        }

        [Test] public void VisitsRestOfRows() {
            new Traverse<Cell>()
                .Rows.Rest(Visit)
                .VisitTable(new CellTree("fixture", "row1", "row2"));
            Assert.AreEqual("row2", nodesVisited.Join(","));
        }

        void Visit(Tree<Cell> tree) { nodesVisited.Add(tree.Value.Text); }

        List<string> nodesVisited;
    }
}
