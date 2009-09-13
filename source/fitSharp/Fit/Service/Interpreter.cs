// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public interface Interpreter {
        CellProcessor Processor { set; }
        void Prepare(Interpreter parent, Tree<Cell> table);
        bool IsInFlow(int tableCount);
        bool IsVisible { get; }
        void Interpret(Tree<Cell> table);
        TestStatus TestStatus { get; }
    }

    public interface FlowInterpreter {
        void InterpretFlow(Tree<Cell> table);
        void DoSetUp(Tree<Cell> table);
        void DoTearDown(Tree<Cell> table);
    }
}
