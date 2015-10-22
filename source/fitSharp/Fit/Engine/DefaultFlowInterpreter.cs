// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public class DefaultFlowInterpreter: FlowInterpreter {
        public DefaultFlowInterpreter(object systemUnderTest) {
            SetSystemUnderTest(systemUnderTest);
        }

        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            new InterpretFlow(processor, this).DoTableFlow(table, 1);
        }

        public object SystemUnderTest { get; private set; }
        public bool IsInFlow(int tableCount) { return tableCount == 1; }
        public void DoSetUp(CellProcessor processor, Tree<Cell> table) {}
        public void DoTearDown(Tree<Cell> table) {}

        public void SetSystemUnderTest(object theSystemUnderTest) {
            SystemUnderTest = theSystemUnderTest;
        }

        public MethodRowSelector MethodRowSelector {
            get { return new DoRowSelector(); }
        }
    }
}
