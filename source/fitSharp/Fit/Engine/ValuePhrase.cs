// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public class ValuePhrase {
        public ValuePhrase(Tree<Cell> cells) { this.cells = cells; }

        public object Evaluate(CellProcessor processor) {
            var symbolName = cells.ValueAt(1).Text;
            if (processor.Get<Symbols>().HasValue(symbolName)) {
                return processor.Get<Symbols>().GetValue(symbolName);
            }
            var interpreter = processor.CallStack.DomainAdapter.GetValue<FlowInterpreter>();
            return interpreter.ExecuteFlowRowMethod(processor, cells);
        }

        readonly Tree<Cell> cells;
    }
}
