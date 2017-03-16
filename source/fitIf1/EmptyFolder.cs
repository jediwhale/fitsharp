using System.Collections.Generic;

namespace fitIf {
    public class EmptyFolder: Folder {
        public EmptyFolder(string path) { Path = path; }
        public Collection<Folder> Folders { get { return new EmptyCollection<Folder>(); } }
        public Collection<Page> Pages { get { return new EmptyCollection<Page>(); } }
        public string Path { get; private set; }

        class EmptyCollection<T> : Collection<T> {
            public IEnumerable<T> Items { get { yield break; } }
            public bool Contains(string name) { return false; }
            public T this[string name] { get { return default(T); } }
        }
    }
}
