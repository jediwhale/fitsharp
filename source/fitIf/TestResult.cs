using fitSharp.Fit.Model;

namespace fitIf {
    public class TestResult {
        public TestResult(TextDictionary textDictionary, TestFile file) {
            this.file = file;
            if (file.IsFolder) return;

            var path = file.FullName + ".html";
            if (!textDictionary.Contains(path)) return;

            using (var input = textDictionary.Reader(path)) {
                var line = input.ReadLine();
                if (line.Length < 8) return;
                var content = line.Substring(4, line.Length - 7).Split(',');
                if (content.Length < 5) return;
                runTime = content[0];
                counts = new TestCounts();
                counts.SetCount(TestStatus.Right, int.Parse(content[1]));
                counts.SetCount(TestStatus.Wrong, int.Parse(content[2]));
                counts.SetCount(TestStatus.Ignore, int.Parse(content[3]));
                counts.SetCount(TestStatus.Exception, int.Parse(content[4]));
            }
        }

        public string Description {
            get {
                return file.IsFolder 
                    ? string.Empty
                    : counts != null
                        ? "Last run: " + runTime + " " + counts.Description
                        : "Last run: never";
            }
        }

        public string Status { get { return counts != null ? counts.Style : TestStatus.Ignore; } }

        readonly TestFile file;
        readonly string runTime;
        readonly TestCounts counts;
    }
}
