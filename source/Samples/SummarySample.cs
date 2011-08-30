using System.Threading;
using fitlibrary;

namespace fitSharp.Samples_1 {
    public class SummarySample: DoFixture {

        public void AddLink() {
            TestStatus.Summary["mylink"] = "<a href=\"somewhere\">somewhere</a>";
        }

        public void Slow() {
            Thread.Sleep(2000);
        }
    }
}
