using System.Collections.Generic;
using fitIf;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.FitIf {
    [TestFixture]
    public class TestResultTest {

        [Test]
        public void FolderHasEmptyResult() {
            var result = new TestResult(new TestFolder(), new TestFile {IsFolder = true});
            Assert.AreEqual(string.Empty, result.Description);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Ignore, result.Status);
        }

        [Test]
        public void TestWithoutResultFileHasEmptyResult() {
            var result = new TestResult(new TestFolder(), new TestFile {FileName="something"});
            Assert.AreEqual("Last run: never", result.Description);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Ignore, result.Status);
        }

        [Test]
        public void ParsesResultFromResultFile() {
            AssertParses("<!--2016-01-02 13:14:15,0,1,0,0-->");
        }

        [Test]
        public void ParsesResultWithPrecedingLines() {
            AssertParses("<!doctype html>\r\n<!--2016-01-02 13:14:15,0,1,0,0-->");
        }

        [Test]
        public void ParsesResultWithTrailingComment() {
            AssertParses("<!--2016-01-02 13:14:15,0,1,0,0--><!--more-->");
        }
         
        static void AssertParses(string contents) {
            var testFolder = new TestFolder {
                PageList = {
                    Pages = new List<Page> {
                        new TestPage {Name = "something.html", Contents = contents}
                    }
                }
            };
            var result = new TestResult(testFolder, new TestFile {FileName = "something"});
            Assert.AreEqual("Last run: 2016-01-02 13:14:15 0 right, 1 wrong, 0 ignored, 0 exceptions", result.Description);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Wrong, result.Status);
        }
    }
}
