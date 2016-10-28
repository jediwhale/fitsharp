using System.Collections.Generic;
using System.IO;

namespace fitIf {
    public interface Folder {
        Collection<Folder> Folders { get; }
        Collection<Page> Pages { get; }
        string Path { get; }
    }

    public interface Collection<out T> {
        IEnumerable<T> Items { get; }
        bool Contains(string name);
        T this[string name] { get; }
    }

    public interface Page {
        string Name { get; }
        TextReader Reader { get; }
    }

    public static class FolderExtension {
        public static string Name(this Folder folder) {
            return Path.GetFileName(folder.Path);
        }

        public static string PathFor(this Folder folder, string name) {
            return Path.Combine(folder.Path, name);
        }

        public static Folder SubFolder(this Folder folder, string name) {
            return folder.Folders.Contains(name)
                ? folder.Folders[name]
                : new EmptyFolder(folder.PathFor(name));
        }

        public static string Content(this Page page) {
            using (var reader = page.Reader) {
                return reader.ReadToEnd();
            }
        }
    }
}
