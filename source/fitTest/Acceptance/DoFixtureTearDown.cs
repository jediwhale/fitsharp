using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoFixtureTearDown: DoFixture {
        public string Message = "TearDown Worked.";
        protected void tearDown() {
            throw new ApplicationException(Message);
        }
        public void anException() {
            throw new ApplicationException("ex");
        }

    }
}