// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public class FlowRow {

        public FlowRow(Tree<Cell> row) {
            this.row = row;
        }

        public void Evaluate(CellProcessor processor, FlowInterpreter interpreter) {
            var specialActionName = processor.ParseTree<Cell, MemberName>(row.Branches[0]);
            var result = processor.Operate<InvokeSpecialOperator>(new TypedValue(interpreter), specialActionName, row.Branches[0]);
            if (result.IsValid) {
                row.ValueAt(0).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxKeyword);
                return;
            }

            if (!result.IsValid) {
                processor.Execute(interpreter,
                        interpreter.MethodRowSelector.SelectMethodCells(row),
                        interpreter.MethodRowSelector.SelectParameterCells(row),
                        row.ValueAt(0));
            }
        }

        readonly Tree<Cell> row;
    }
}
