using fitSharp.TestTarget;

namespace fitSharp.TestTarget2 {
    public class SampleWithDependency {
        public SampleDomain Sample = new SampleDomain();
        public override string ToString() {
            return "my sample says " + Sample;
        }
    }
}
