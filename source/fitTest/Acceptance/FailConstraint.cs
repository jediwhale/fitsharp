using fitlibrary;

namespace fit.Test.Acceptance {
    public class FailConstraint: ConstraintFixture {
        public FailConstraint(): base(false) {}
        public bool BA(int b, int a) {
            return a < b;
        }
    }
}