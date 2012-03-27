// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public interface Interpreter {
        void Interpret(CellProcessor processor, Tree<Cell> table);
    }

    public interface FlowInterpreter: Interpreter, MutableDomainAdapter {
        bool IsInFlow(int tableCount);
        void DoSetUp(CellProcessor processor, Tree<Cell> table);
        void DoTearDown(Tree<Cell> table);
        MethodRowSelector MethodRowSelector { get; }
    }

    public interface InterpretTableFlow {
        void DoTableFlow(Tree<Cell> table, int rowsToSkip);
    }

    public static class FlowInterpreterExtension {
        public static object ExecuteFlowRowMethod(this FlowInterpreter interpreter, CellProcessor processor, Tree<Cell> row) {
            try {
                var cells = row.Skip(1);
                return
                    processor.ExecuteWithThrow(interpreter,
                            interpreter.MethodRowSelector.SelectMethodCells(cells),
                            interpreter.MethodRowSelector.SelectParameterCells(cells),
                            row.ValueAt(1)).
                        Value;
            }
            catch (ParseException<Cell> e) {
                processor.TestStatus.MarkException(e.Subject, e.InnerException);
                throw new IgnoredException();
            }
        }
    }
}
