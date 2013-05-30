// Copyright © 2013 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using NUnit.Framework;
using fitSharp.Fit.Model;
using fitSharp.Fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Samples;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class StoryTestFileTest {
        [Test] public void DecoratesResultWithNoStyleSheetLink() {
            CheckResult("content", "<link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\">content");
        }

        [Test] public void DecoratesResultWithOtherStyleSheetLink() {
            CheckResult("<link other stuff>content", "<link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\"><link other stuff>content");
        }

        [Test] public void DoesNotDecorateWithFitLink() {
            const string content = "<link type=\"text/css\" href=\"fit.css\" rel=\"stylesheet\">content";
            CheckResult(content, content);
        }

        [Test] public void DoesNotDecoratesWithMultipleLinks() {
            const string content = "<link other stuff><link type=\"text/css\" href=\"fit.css\" rel=\"stylesheet\">content";
            CheckResult(content, content);
        }

        static void CheckResult(string content, string expected) {
            var folder = new FolderTestModel();
            var memory = new TypeDictionary();
            memory.GetItem<Settings>().OutputFolder = "output";
            var file = new StoryTestFile("myfile", new StoryTestFolder(memory, folder), folder);
            file.WriteTest(new PageResult("title", content, new TestCounts()));
            Assert.AreEqual(expected, folder.GetPageContent(new FilePath("output\\myfile.html")));
        }
    }
}
