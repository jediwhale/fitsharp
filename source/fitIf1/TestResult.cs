using System;
using fitSharp.Fit.Model;
using fitSharp.Parser;

namespace fitIf {
    public class TestResult {
        public TestResult(Folder folder, TestFile file) {
            this.file = file;
            if (file.IsFolder) return;

            var path = file.FileName + ".html";
            if (!folder.Pages.Contains(path)) return;

            using (var input = folder.Pages[path].Reader) {
                while (true) {
                    var line = input.ReadLine();
                    if (line == null || line.Length < 8 || !line.StartsWith("<!", StringComparison.Ordinal)) return;
                    var scanner = new Scanner(line);
                    scanner.FindTokenPair("<!--", "-->");
                    var content = scanner.Body.ToString().Split(',');
                    if (content.Length < 5) continue;
                    runTime = content[0];
                    counts = new TestCounts();
                    counts.SetCount(TestStatus.Right, int.Parse(content[1]));
                    counts.SetCount(TestStatus.Wrong, int.Parse(content[2]));
                    counts.SetCount(TestStatus.Ignore, int.Parse(content[3]));
                    counts.SetCount(TestStatus.Exception, int.Parse(content[4]));
                    break;
                }
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
