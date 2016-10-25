using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fitIf {
    public class FileFolder: Folder {
        public FileFolder(string path) {
            Path = path;
        }

        public string Path { get; private set; }

        public IEnumerable<string> PageNames {
            get { return Directory.EnumerateFiles(Path);}
        }
    }

    public class FileSystemTree: BasicTree<Folder> {
        public FileSystemTree(string path) {
            this.path = path;
        }

        public Folder Value { get { return new FileFolder(path); } }

        public IEnumerable<BasicTree<Folder>> Branches {
            get { return Directory.EnumerateDirectories(path).Select(subPath => new FileSystemTree(subPath)); }
        }

        readonly string path;
    }

    public class FileSystem: TextDictionary {

        public FileSystem(string root) {
            this.root = root;
        }

        public bool Contains(string key) {
            return File.Exists(Path.Combine(root, key));
        }

        public TextReader Reader(string key) {
            return new StreamReader(Path.Combine(root, key));
        }

        readonly string root;
    }
}
