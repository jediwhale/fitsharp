using fitSharp.Machine.Model;

namespace fitIf {
    public interface MainView {
        void ShowResult(string path);
        void ShowTests(Tree<TestFile> tests);
    }
}
