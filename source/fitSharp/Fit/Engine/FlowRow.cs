// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public static class FlowRow {

        public static void Evaluate(this Tree<Cell> row, CellProcessor processor) {
            var interpreter = processor.CallStack.DomainAdapter.GetValueAs<FlowInterpreter>();
            if (!row.InvokeSpecialAction(processor, interpreter).IsValid) {
                row.ExecuteMethod(processor, interpreter);
            }
        }

        public static TypedValue InvokeSpecialAction(this Tree<Cell> row, CellProcessor processor, FlowInterpreter interpreter) {
            var specialActionName = processor.ParseTree<Cell, MemberName>(row.Branches[0]);
            var result = processor.Operate<InvokeSpecialOperator>(new TypedValue(interpreter), specialActionName, row);
            return result;
        }

        public static TypedValue ExecuteMethod(this Tree<Cell> row, CellProcessor processor, FlowInterpreter interpreter) {
            return row.ExecuteMethod(processor, interpreter.MethodRowSelector, interpreter);
        }

        public static TypedValue ExecuteMethod(this Tree<Cell> row, CellProcessor processor, MethodRowSelector selector,
                                               object target) {
            return processor.Execute(target,
                        selector.SelectMethodCells(row),
                        selector.SelectParameterCells(row),
                        row.ValueAt(0));
        }
    }
}
