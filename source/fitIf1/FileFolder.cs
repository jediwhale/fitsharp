using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fitIf {
    public class FileFolder: Folder {
        public FileFolder(string path) {
            Path = path;
            Folders = new FolderItems(path);
            Pages = new PageItems(path);
        }

        public Collection<Folder> Folders { get; private set; }
        public Collection<Page> Pages { get; private set; }
        public string Path { get; private set; }

        class FolderItems: Collection<Folder> {
            public FolderItems(string path) {
                this.path = path;
            }

            public IEnumerable<Folder> Items {
                get { return Directory.EnumerateDirectories(path).Select(subPath => new FileFolder(subPath)); }
            }

            public bool Contains(string name) {
                return Directory.Exists(System.IO.Path.Combine(path, name));
            }

            public Folder this[string name] {
                get { return new FileFolder(System.IO.Path.Combine(path, name)); }
            }

            readonly string path;
        }

        class PageItems: Collection<Page> {
            public PageItems(string path) {
                this.path = path;
            }

            public IEnumerable<Page> Items {
                get { return Directory.EnumerateFiles(path).Select(filePath => new FilePage(filePath)); }
            }

            public bool Contains(string name) {
                return File.Exists(System.IO.Path.Combine(path, name));
            }

            public Page this[string name] {
                get { return new FilePage(System.IO.Path.Combine(path, name)); }
            }

            readonly string path;
        }
    }

    public class FilePage : Page {

        public FilePage(string path) {
            this.path = path;
        }

        public string Name { get { return Path.GetFileName(path); } }
        public TextReader Reader { get { return new StreamReader(path); } }

        readonly string path;
    }
}
