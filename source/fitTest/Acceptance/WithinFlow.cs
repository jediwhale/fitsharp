using fitlibrary;

namespace fit.Test.Acceptance {
    public class WithinFlow: DoFixture {
        public DoFixtureSetUp withSetUp() {
            return new DoFixtureSetUp();
        }
        public DoFixtureTearDown withTearDown() {
            return new DoFixtureTearDown();
        }
        public DoFixtureSetUpWithException withSetUpException() {
            return new DoFixtureSetUpWithException();
        }
    }
}