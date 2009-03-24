using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureSetUp: CalculateFixture {
    
        public void setUp() {
            IAmSetUp = true;
        }
        public int resultA(int a) { 
            if (IAmSetUp)
                return a+1;
            return 0;
        }
        private bool IAmSetUp = false;
    }
}