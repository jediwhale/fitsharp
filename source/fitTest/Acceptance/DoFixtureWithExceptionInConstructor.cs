using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoFixtureWithExceptionInConstructor: DoFixture {
        public DoFixtureWithExceptionInConstructor() {
            throw new ApplicationException("Bad constructor");
        }
    }
}