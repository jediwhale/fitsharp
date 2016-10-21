namespace fitIf {
    public class TestFile {
        public string FileName;
        public string Path;
        public string TestStatus;
        public bool IsFolder;

        public string FullName {
            get { return string.IsNullOrEmpty(Path) ? FileName : System.IO.Path.Combine(Path, FileName); }
        }

        public string DisplayName {
            get { return string.IsNullOrEmpty(FileName) ? "Tests" : FileName; }
        }
    }
}
