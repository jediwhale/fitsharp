// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class ConfigureFixture: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            processor.TestStatus.TableCount--;

            var facility = processorIdentifier.Matches(table.ValueAt(0, 1).Text)
                ? processor
                : processor.Memory.GetItem(table.ValueAt(0, 1).Text);

            table.ValueAt(0, 1).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxSUT);

            if (table.Branches[0].Branches.Count > 2) {
                var currentRow = table.Branches[0].Skip(2);
                Execute(processor, facility, currentRow);
            }

            new Traverse<Cell>()
                .Rows.All(row => Execute(processor, facility, row))
                .VisitTable(table);
        }

        static void Execute(CellProcessor processor, object facility, Tree<Cell> currentRow) {
            var selector = new DoRowSelector();
            var result = processor.ExecuteWithThrow(facility, selector.SelectMethodCells(currentRow),
                                                    selector.SelectParameterCells(currentRow),
                                                    currentRow.ValueAt(0));
            if (result.IsVoid) return;
            currentRow.ValueAt(0).SetAttribute(CellAttribute.Folded, result.ValueString);
        }

        readonly IdentifierName processorIdentifier = new IdentifierName("processor");
    }
}
