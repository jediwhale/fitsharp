using fitSharp.Fit.Model;

namespace fitIf {
    public class TestResult {
        public TestResult(Folder folder, TestFile file) {
            this.file = file;
            if (file.IsFolder) return;

            var path = file.FileName + ".html";
            if (!folder.Pages.Contains(path)) return;

            using (var input = folder.Pages[path].Reader) {
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

        public string Status {
            get {
                if (counts == null) return TestStatus.Ignore;
                var style = counts.Style;
                return string.IsNullOrEmpty(style) ? TestStatus.Right : style;
            }
        }

        readonly TestFile file;
        readonly string runTime;
        readonly TestCounts counts;
    }
}
