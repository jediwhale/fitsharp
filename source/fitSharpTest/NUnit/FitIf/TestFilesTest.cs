using System;
using System.Collections.Generic;
using fitIf;
using fitSharp.Fit.Application;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.FitIf {
    [TestFixture]
    public class TestFilesTest {

        [SetUp]
        public void SetUp() {
            testFolder = new TestFolder { Path = "root\\Tests"};
            resultFolder = new TestFolder { Path = "other\\Results"};
            testFiles = new TestFiles(testFolder, resultFolder, new StoryTestSuite(new FileExclusions(), s => true));
        }

        [Test]
        public void CreatesTree() {
            testFolder.PageList.Pages = new List<Page> {
                new TestPage {Name = "leaf1.html", Contents = "stuff"},
                new TestPage {Name = "leaf2.html", Contents = "stuff"}
            };
            resultFolder.PageList.Pages = new List<Page> {
                new TestPage {Name = "leaf1.html", Contents = "<!doctype html>\r\n<!--2016-01-02 13:14:15,1,0,0,0-->" + Environment.NewLine + "more stuff"},
                new TestPage {Name = "leaf2.html", Contents = "<!--2016-01-02 13:14:15,0,1,0,0-->" + Environment.NewLine + "more stuff"}
            };
            var tree = testFiles.Tree;
            Assert.AreEqual(",,True,[ ,leaf1,False,pass[ ] ,leaf2,False,fail[ ] ]", tree.List(Show));
        }

        [Test]
        public void CreatesNestedFolder() {
            testFolder.PageList.Pages = new List<Page> {new TestPage {Name = "leaf1.html", Contents = "stuff"}};
            var subFolder = new TestFolder {
                Path = "root\\Tests\\subfolder",
                PageList = {Pages = new List<Page> {new TestPage {Name = "leaf2.html", Contents = "stuff"}}}
            };
            testFolder.FolderList.Folders = new List<Folder> {subFolder};
            var tree = testFiles.Tree;
            Assert.AreEqual(",,True,[ ,leaf1,False,ignore[ ] ,subfolder,True,[ subfolder,leaf2,False,ignore[ ] ] ]", tree.List(Show));
        }

        [Test]
        public void ChecksResultInNestedFolder() {
            testFolder.PageList.Pages = new List<Page> {new TestPage {Name = "leaf1.html", Contents = "stuff"}};
            var subFolder = new TestFolder {
                Path = "root\\Tests\\subfolder",
                PageList = {Pages = new List<Page> {new TestPage {Name = "leaf2.html", Contents = "stuff"}}}
            };
            testFolder.FolderList.Folders = new List<Folder> {subFolder};
            var resultSubFolder = new TestFolder {
                Path = "other\\Results\\subfolder",
                PageList = {Pages = new List<Page> {new TestPage {Name = "leaf2.html", Contents = "<!--2016-01-02 13:14:15,0,1,0,0-->" + Environment.NewLine}}}
            };
            resultFolder.FolderList.Folders = new List<Folder> {resultSubFolder};
            var tree = testFiles.Tree;
            Assert.AreEqual(",,True,[ ,leaf1,False,ignore[ ] ,subfolder,True,[ subfolder,leaf2,False,fail[ ] ] ]", tree.List(Show));
        }

        [Test]
        public void HandlesMissingFolder() {
            var tree = testFiles.Tree;
            Assert.AreEqual(",,True,[ ]", tree.List(Show));
        }

        static string Show(TestFile file) {
            return string.Format("{0},{1},{2},{3}", file.Path, file.FileName, file.IsFolder, file.TestStatus);
        }

        TestFolder testFolder;
        TestFolder resultFolder;
        TestFiles testFiles;
    }
}
