// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class Define: Interpreter {
        public TestStatus TestStatus { get; private set; }

        private CellProcessor processor;

        public Define() { TestStatus = new TestStatus(); }

        public CellProcessor Processor {
            set { processor = value; }
        }

        public bool IsInFlow(int tableCount) { return false; }

        public bool IsVisible { get { return false; } }

        public void Interpret(Tree<Cell> table) {
            var body = new TreeList<Cell>();
            for (int i = 1; i < table.Branches.Count; i++) {
                body.AddBranch(table.Branches[i]);
            }
            processor.Store(new Procedure(table.Branches[0].Branches[1].Value.Text.Trim(), new CellTree(body)));
        }
    }
}
