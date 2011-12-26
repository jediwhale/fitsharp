// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class Compute: Interpreter {

        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            new Traverse<Cell>()
                .Rows.Header(row => headerRow = row)
                .Rows.Rest(row => ComputeRow(processor, row))
                .VisitTable(table);
        }

        void ComputeRow(CellProcessor processor, Tree<Cell> row) {
            var memberName = processor.ParseTree<Cell, MemberName>(headerRow.Branches.Last()).WithNamedParameters();

            var parameterList = new List<Tree<Cell>>();
            for (var i = 0; i < headerRow.Branches.Count - 1; i++) {
                parameterList.Add(new CellTreeLeaf(new GracefulName(headerRow.Branches[i].Value.Text).ToString()));
                parameterList.Add(row.Branches[i]);
            }
            var result = processor.Invoke(processor.CallStack.SystemUnderTest, memberName, 
                                          new EnumeratedTree<Cell>(parameterList));
            processor.Check(result, row.Branches.Last());
        }

        Tree<Cell> headerRow;
    }
}
