namespace fitSharp.Model {
    public interface TreeWriter<T> {
        void WritePrefix(Tree<T> tree);
        void WriteBranch(Tree<T> tree, int index);
        void WriteSuffix(Tree<T> tree);
    }
}