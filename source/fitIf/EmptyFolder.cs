using System.Collections.Generic;
using System.IO;

namespace fitIf {
    public class EmptyFolder : Folder {
        public EmptyFolder(string path) { Path = path; }
        public bool Contains(string key) { return false; }
        public TextReader Reader(string key) { return null; }
        public IEnumerable<string> Pages { get { yield break; } }
        public IEnumerable<Folder> Folders { get { yield break; } }
        public string Path { get; private set; }
    }
}
