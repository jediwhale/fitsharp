using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class CalculateFixtureTearDown: CalculateFixture {
        protected void tearDown() {
            throw new ApplicationException("TearDown Worked.");
        }
        public int resultA(int a) { 
            throw new ApplicationException("ex"+a);
        }
    }
}