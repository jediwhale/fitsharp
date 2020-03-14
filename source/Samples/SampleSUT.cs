using fitSharp.Fit.Exception;

namespace fitSharp.Samples {
    public class SampleSUT {
        
        public string ThrowAbandonSuite() {
            throw new AbandonSuiteException();
        }

    }
}
