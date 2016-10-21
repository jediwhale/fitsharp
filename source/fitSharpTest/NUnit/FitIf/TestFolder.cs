using System.Collections.Generic;
using System.IO;
using fitIf;

namespace fitSharp.Test.NUnit.FitIf {
    public class TestFolder: Folder {
        public bool Contains(string key) {
            return PageList.ContainsKey(key);
        }

        public TextReader Reader(string key) {
            return new StringReader(PageList[key]);
        }

        public IEnumerable<string> Pages { get { return PageList.Keys; } }

        public IEnumerable<Folder> Folders { get { return FolderList; } }

        public string Path { get; set; }

        public readonly Dictionary<string, string> PageList = new Dictionary<string, string>();
        public readonly List<Folder> FolderList = new List<Folder>();
    }
}
