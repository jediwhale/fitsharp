using fitlibrary;

namespace fitSharp.Samples {
    public class SummarySample: DoFixture {

        public void AddLink() {
            TestStatus.Summary["mylink"] = "<a href=\"somewhere\">somewhere</a>";
        }
    }
}
