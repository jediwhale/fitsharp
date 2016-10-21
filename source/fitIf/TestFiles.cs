using System.IO;
using System.Linq;
using fitSharp.Machine.Model;

namespace fitIf {
    public class TestFiles {
        public TestFiles(Folder testRootFolder, Folder resultRootFolder) {
            this.testRootFolder = testRootFolder;
            this.resultRootFolder = resultRootFolder;
        }

        public Tree<TestFile> Tree {
            get {
                var tree = new TreeList<TestFile>(new TestFile { FileName = string.Empty, IsFolder = true});
                AddFiles(tree, testRootFolder);
                return tree;
            }
        }

        void AddFiles(TreeList<TestFile> tree, Folder folder) {
            foreach (var file in folder.Pages.Where(file => Path.GetExtension(file) == ".html")) {
                var testFile = new TestFile {
                    FileName = Path.GetFileNameWithoutExtension(file), Path = tree.Value.FullName
                };
                var result = new TestResult(resultRootFolder, testFile);
                testFile.TestStatus = result.Status;
                tree.Add(testFile);
            }
            foreach (var subfolder in folder.Folders) {
                var branch = new TreeList<TestFile>(new TestFile {FileName = subfolder.Name(), Path = tree.Value.FullName, IsFolder = true});
                tree.Add(branch);
                AddFiles(branch, subfolder);
            }
        }

        readonly Folder testRootFolder;
        private readonly Folder resultRootFolder;
    }
}
