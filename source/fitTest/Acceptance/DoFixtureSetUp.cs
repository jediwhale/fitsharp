using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoFixtureSetUp: DoFixture {
        private bool IAmSetUp = false;
    
        public void setUp() {
            IAmSetUp = true;
        }
        public bool isSetUp() {
            return IAmSetUp ;
        }
    }
}