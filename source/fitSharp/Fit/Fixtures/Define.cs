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

        public void Prepare(Interpreter parent, Tree<Cell> table) {}

        public bool IsInFlow(int tableCount) { return false; }

        public bool IsVisible { get { return false; } }

        public void Interpret(Tree<Cell> table) {}
    }
}
