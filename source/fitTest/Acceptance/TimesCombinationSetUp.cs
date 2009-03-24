using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class TimesCombinationSetUp: CombinationFixture {
    
        public void SetUp() {
            isSetUp = true;
        }

        public void TearDown() {
            throw new ApplicationException("tear down");
        }

        public int Combine(int x, int y) {
            if (!isSetUp)
                throw new ApplicationException("Not set up");
            return x * y;
        }

        public TimesCombinationSetUp DoIt() {
            return new TimesCombinationSetUp();
        }

        private bool isSetUp = false;
    }
}