// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class MatchValue: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            new Traverse<Cell>()
                .Rows.First(row => selectValue = new ValuePhrase(row).Evaluate(processor))
                .Rows.All(row => SelectRow(processor, row))
                .VisitTable(table);

        }

        void SelectRow(CellProcessor processor, Tree<Cell> row) {
            if (processor.Compare(new TypedValue(selectValue), row.Branches[0])) {
                row.Skip(1).Evaluate(processor);
            }
            else {
                row.Value.SetAttribute(CellAttribute.Status, TestStatus.Ignore);
            }
        }

        object selectValue;
    }
}
