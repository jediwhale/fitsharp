using System.IO;
using System.Linq;
using fitSharp.Machine.Model;

namespace fitIf {
    public class TestFiles {
        public TestFiles(BasicTree<Folder> testRootFolder, TextDictionary resultTextDictionary) {
            this.testRootFolder = testRootFolder;
            this.resultTextDictionary = resultTextDictionary;
        }

        public Tree<TestFile> Tree {
            get {
                var tree = new TreeList<TestFile>(new TestFile { FileName = string.Empty, IsFolder = true});
                AddFiles(tree, testRootFolder);
                return tree;
            }
        }

        void AddFiles(TreeList<TestFile> tree, BasicTree<Folder> folder) {
            foreach (var file in folder.Value.PageNames.Where(file => Path.GetExtension(file) == ".html")) {
                var testFile = new TestFile {
                    FileName = Path.GetFileNameWithoutExtension(file), Path = tree.Value.FullName
                };
                var result = new TestResult(resultTextDictionary, testFile);
                testFile.TestStatus = result.Status;
                tree.Add(testFile);
            }
            foreach (var subfolder in folder.Branches) {
                var branch = new TreeList<TestFile>(new TestFile {FileName = subfolder.Value.Name(), Path = tree.Value.FullName, IsFolder = true});
                tree.Add(branch);
                AddFiles(branch, subfolder);
            }
        }

        readonly BasicTree<Folder> testRootFolder;
        readonly TextDictionary resultTextDictionary;
    }
}
