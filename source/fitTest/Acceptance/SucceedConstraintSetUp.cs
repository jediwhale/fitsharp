using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class SucceedConstraintSetUp: ConstraintFixture {
        public void SetUp() {
            IAmSetUp = true;
        }
        public void TearDown() {
            throw new ApplicationException("tear down");
        }
        public bool AB(int a, int b) {
            return IAmSetUp && a < b;
        }
        public SucceedConstraintSetUp DoIt() {
            return new SucceedConstraintSetUp();
        }
        private bool IAmSetUp = false;
    }
}