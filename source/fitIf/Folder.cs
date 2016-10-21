using System.Collections.Generic;
using System.IO;

namespace fitIf {
    public interface Folder {
        bool Contains(string key);
        TextReader Reader(string key);
        IEnumerable<string> Pages { get; }
        IEnumerable<Folder> Folders { get; }
        string Path { get; }
    }

    public static class FolderExtension {
        public static string Name(this Folder folder) {
            return Path.GetFileName(folder.Path);
        }
    }

    public interface BasicTree<T> {
        T Value { get; }
        IEnumerable<T> Branches { get; }
    }
}
