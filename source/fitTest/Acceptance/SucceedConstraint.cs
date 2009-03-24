using fitlibrary;

namespace fit.Test.Acceptance {
    public class SucceedConstraint: ConstraintFixture {
        public SucceedConstraint() {
            RepeatString = "ditto";
        }
        public bool aB(int a, int b) {
            return a < b;
        }
        public int bC(int b, int c) {
            return b + c;
        }
    }
}