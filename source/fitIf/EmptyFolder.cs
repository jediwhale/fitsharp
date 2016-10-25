using System.Collections.Generic;

namespace fitIf {
    public class EmptyFolder: Folder {
        public EmptyFolder(string path) { Path = path; }
        public IEnumerable<string> PageNames { get { yield break; } }
        public string Path { get; private set; }
    }
}
