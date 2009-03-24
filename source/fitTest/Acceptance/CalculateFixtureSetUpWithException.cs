using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureSetUpWithException: CalculateFixture {
        public void setUp() {
            throw new ApplicationException("setUp exception.");
        }
        public int resultA(int a) { 
            return a+1;
        }
    }
}