using System.Collections.Generic;
using System.IO;
using System.Linq;
using fitIf;

namespace fitSharp.Test.NUnit.FitIf {
    public class TestFolder: Folder {
        public TestFolderList FolderList = new TestFolderList();
        public TestPageList PageList = new TestPageList();
        public Collection<Folder> Folders { get { return FolderList; } }
        public Collection<Page> Pages { get { return PageList; } }
        public string Path { get; set; }
    }

    public class TestFolderList : Collection<Folder> {
        public List<Folder> Folders = new List<Folder>(); 
        public IEnumerable<Folder> Items { get { return Folders; } }
        public bool Contains(string name) { return Folders.Any(folder => folder.Name() == name); }
        public Folder this[string name] { get { return Folders.First(folder => folder.Name() == name); } }
    }

    public class TestPageList : Collection<Page> {
        public List<Page> Pages = new List<Page>(); 
        public IEnumerable<Page> Items { get { return Pages; }}
        public bool Contains(string name) { return Pages.Any(page => page.Name == name); }
        public Page this[string name] { get { return Pages.First(page => page.Name == name); } }
    }

    public class TestPage: Page {
        public string Contents;
        public string Name { get; set; }
        public TextReader Reader { get { return new StringReader(Contents);} }
    }
}
