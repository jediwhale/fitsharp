using fitlibrary;

namespace fit.Test.Acceptance {
    public class MixedCombination: CombinationFixture {
        public bool Combine(string s, int x) {
            return x == 1;
        }
    }
}