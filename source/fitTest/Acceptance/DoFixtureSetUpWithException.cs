using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoFixtureSetUpWithException: DoFixture {
        public void setUp() {
            throw new ApplicationException("setUp exception.");
        }
    }
}