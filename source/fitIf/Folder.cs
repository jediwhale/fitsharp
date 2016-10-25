using System.Collections.Generic;
using System.IO;

namespace fitIf {
    public interface TextDictionary {
        bool Contains(string key);
        TextReader Reader(string key);
    }

    public interface Folder {
        IEnumerable<string> PageNames { get; }
        string Path { get; }
    }

    public static class FolderExtension {
        public static string Name(this Folder folder) {
            return Path.GetFileName(folder.Path);
        }
    }

    public interface BasicTree<out T> {
        T Value { get; }
        IEnumerable<BasicTree<T>> Branches { get; }
    }
}
