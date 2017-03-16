using System.IO;
using fitSharp.Machine.Model;

namespace fitIf {
    public class TestFiles {
        public TestFiles(Folder testRootFolder, Folder resultRootFolder, StoryTestSuite suite) {
            this.testRootFolder = testRootFolder;
            this.resultRootFolder = resultRootFolder;
            this.suite = suite;
        }

        public Tree<TestFile> Tree {
            get {
                var tree = new TreeList<TestFile>(new TestFile { FileName = string.Empty, IsFolder = true});
                AddFiles(tree, testRootFolder, resultRootFolder);
                return tree;
            }
        }

        void AddFiles(TreeList<TestFile> tree, Folder testFolder, Folder resultFolder) {
            foreach (var name in suite.TestNames(testFolder)) {
                var testFile = new TestFile {
                    FileName = Path.GetFileNameWithoutExtension(name), Path = tree.Value.FullName
                };
                var result = new TestResult(resultFolder, testFile);
                testFile.TestStatus = result.Status;
                tree.Add(testFile);
            }

            foreach (var subfolder in suite.SubFolders(testFolder)) {
                var branch = new TreeList<TestFile>(new TestFile {FileName = subfolder.Name(), Path = tree.Value.FullName, IsFolder = true});
                tree.Add(branch);
                AddFiles(branch, subfolder, resultFolder.SubFolder(subfolder.Name()));
            }
        }

        readonly Folder testRootFolder;
        readonly Folder resultRootFolder;
        readonly StoryTestSuite suite;
    }
}
