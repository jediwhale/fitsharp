using System.Collections.Generic;
using System.IO;
using fitIf;

namespace fitSharp.Test.NUnit.FitIf {
    public class TestFolder: Folder {
        public IEnumerable<string> PageNames { get { return PageList; } }
        public string Path { get; set; }
        public List<string> PageList = new List<string>();
    }

    public class TestTextDictionary: TextDictionary {
        public bool Contains(string key) {
            return PageList.ContainsKey(key);
        }

        public TextReader Reader(string key) {
            return new StringReader(PageList[key]);
        }

        public readonly Dictionary<string, string> PageList = new Dictionary<string, string>();
    }

    public class TestFolderTree : BasicTree<Folder> {
        public Folder Value { get; set; }
        public IEnumerable<BasicTree<Folder>> Branches { get { return FolderList; } }
        public readonly List<BasicTree<Folder>> FolderList = new List<BasicTree<Folder>>();
    }
}
