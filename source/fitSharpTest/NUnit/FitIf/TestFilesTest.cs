using System;
using fitIf;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.FitIf {
    [TestFixture]
    public class TestFilesTest {

        [SetUp]
        public void SetUp() {
            testFolder = new TestFolder { Path = "root\\Tests"};
            testFolderTree = new TestFolderTree { Value = testFolder };
            textDictionary = new TestTextDictionary();
            
        }

        [Test]
        public void CreatesTree() {
            testFolder.PageList.Add("leaf1.html");
            testFolder.PageList.Add("leaf2.html");
            textDictionary.PageList.Add("leaf1.html", "<!--2016-01-02 13:14:15,1,0,0,0-->" + Environment.NewLine + "more stuff");
            textDictionary.PageList.Add("leaf2.html", "<!--2016-01-02 13:14:15,0,1,0,0-->" + Environment.NewLine + "more stuff");
            var tree = new TestFiles(testFolderTree, textDictionary).Tree;
            Assert.AreEqual(",,True,[ ,leaf1,False,pass[ ] ,leaf2,False,fail[ ] ]", tree.List(Show));
        }

        [Test]
        public void CreatesNestedFolder() {
            testFolder.PageList.Add("leaf1.html");
            var subfolder = new TestFolder {Path = "root\\Tests\\subfolder"};
            subfolder.PageList.Add("leaf2.html");
            testFolderTree.FolderList.Add(new TestFolderTree { Value = subfolder});
            var tree = new TestFiles(testFolderTree, textDictionary).Tree;
            Assert.AreEqual(",,True,[ ,leaf1,False,ignore[ ] ,subfolder,True,[ subfolder,leaf2,False,ignore[ ] ] ]", tree.List(Show));
        }

        [Test]
        public void HandlesMissingFolder() {
            var tree = new TestFiles(testFolderTree, textDictionary).Tree;
            Assert.AreEqual(",,True,[ ]", tree.List(Show));
        }

        static string Show(TestFile file) {
            return string.Format("{0},{1},{2},{3}", file.Path, file.FileName, file.IsFolder, file.TestStatus);
        }

        TestFolderTree testFolderTree;
        TestTextDictionary textDictionary;
        TestFolder testFolder;
    }
}
