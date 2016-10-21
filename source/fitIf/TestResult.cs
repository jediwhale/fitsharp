using fitSharp.Fit.Model;

namespace fitIf {
    public class TestResult {
        public TestResult(Folder rootFolder, TestFile file) {
            this.rootFolder = rootFolder;
            this.file = file;
            if (file.IsFolder) return;

            path = file.FullName + ".html";
            if (!this.rootFolder.Contains(path)) return;

            using (var input = this.rootFolder.Reader(path)) {
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

        public string Path {
            get { return !file.IsFolder && rootFolder.Contains(path) ? System.IO.Path.Combine(rootFolder.Path, path) : string.Empty; }
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

        readonly Folder rootFolder;
        readonly TestFile file;
        readonly string path;
        readonly string runTime;
        readonly TestCounts counts;
    }
}
