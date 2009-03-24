using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoFixtureFlowUnderTestWithoutSUT: DoFixture {
        public int plusAB(int a, int b) {
            return a+b;
        }
    }
}