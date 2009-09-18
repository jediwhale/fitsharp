using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
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
