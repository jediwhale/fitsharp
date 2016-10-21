using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fitIf {
    public class FileFolder: Folder {
        public FileFolder(string path) {
            Path = path;
        }

        public string Path { get; private set; }


        public bool Contains(string key) {
            return File.Exists(FilePath(key));
        }

        public TextReader Reader(string key) {
            return new StreamReader(FilePath(key));
        }

        public IEnumerable<string> Pages {
            get { return Directory.EnumerateFiles(Path);}
        }

        public IEnumerable<Folder> Folders {
            get { return Directory.EnumerateDirectories(Path).Select(path => new FileFolder(path)); }
        }

        string FilePath(string key) { return System.IO.Path.Combine(Path, key); }
    }
}
