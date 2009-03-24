using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoCalculateFixtureSetUp: DoFixture {
        public CalculateFixtureSetUp calcSetUp() {
            return new CalculateFixtureSetUp();
        }
        public CalculateFixtureSetUpWithException calculateSetUpWithException() {
            return new CalculateFixtureSetUpWithException();
        }
    }
}