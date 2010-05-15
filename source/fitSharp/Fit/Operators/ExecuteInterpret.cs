// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteInterpret: CellOperator, ExecuteOperator<Cell> {
        public bool CanExecute(TypedValue instance, Tree<Cell> parameters) {
            return instance.IsVoid || typeof (Interpreter).IsAssignableFrom(instance.Type);
        }

        public TypedValue Execute(TypedValue instance, Tree<Cell> parameters) {
            var flowInterpreter = instance.Value as FlowInterpreter;
            foreach (var table in parameters.Branches) flowInterpreter = InterpretTable(flowInterpreter, table);
            return TypedValue.Void;
        }

        private FlowInterpreter InterpretTable(FlowInterpreter flowInterpreter, Tree<Cell> table) {
            if (flowInterpreter == null) {
                var interpreter = Processor.ParseTree<Cell, Interpreter>(table.Branches[0]);
                interpreter.Interpret(table);
                return interpreter as FlowInterpreter;
            }
            flowInterpreter.InterpretFlow(table);
            return flowInterpreter;
        }
    }
}
