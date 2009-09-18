// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteInterpret: CellOperator, ExecuteOperator<Cell> {
        public bool CanExecute(TypedValue instance, Tree<Cell> parameters) {
            return instance.IsVoid || typeof (Interpreter).IsAssignableFrom(instance.Type);
        }

        public TypedValue Execute(TypedValue instance, Tree<Cell> parameters) {
            var flowInterpreter = instance.Value as FlowInterpreter;
            foreach (Tree<Cell> table in parameters.Branches) {
                if (flowInterpreter == null) {
                    var interpreter =
                        Processor.Parse(typeof (Interpreter), TypedValue.Void, table).GetValue<Interpreter>();
                    interpreter.Interpret(table);
                    flowInterpreter = interpreter as FlowInterpreter;
                }
                else {
                    flowInterpreter.InterpretFlow(table);
                }
            }
            return TypedValue.Void;
        }
    }
}
