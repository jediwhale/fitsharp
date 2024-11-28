// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Application;
using NUnit.Framework;
using fitSharp.Fit.Model;
using fitSharp.Fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Samples;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class StoryTestFileTest {
        [SetUp]
        public void SetUp() {
            folder = new FolderTestModel();
        }

        [Test] public void DecoratesResultWithNoStyleSheetLink() {
            CheckResult("content", "<link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\">" + Environment.NewLine + "content");
        }

        [Test] public void DecoratesResultWithOtherStyleSheetLink() {
            CheckResult("<link other stuff>content", "<link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\">" + Environment.NewLine + "<link other stuff>content");
        }

        [Test] public void DoesNotDecorateWithFitLink() {
            const string content = "<link type=\"text/css\" href=\"fit.css\" rel=\"stylesheet\">content";
            CheckResult(content, content);
        }

        [Test] public void DoesNotDecoratesWithMultipleLinks() {
            const string content = "<link other stuff><link type=\"text/css\" href=\"fit.css\" rel=\"stylesheet\">content";
            CheckResult(content, content);
        }

        [Test]
        public void GetsContentForExcelSpreadsheet() {
            var file = MakeStoryTestFile("abc.xlsx");
            ClassicAssert.IsTrue(file.TestContent is ExcelStoryTestSource);
        }

        FolderTestModel folder;

        void CheckResult(string content, string expected) {
            Clock.Instance = new TestClock {Now = new DateTime(2016, 1, 2, 13, 14, 15)};
            var file = MakeStoryTestFile("myfile");
            file.WriteTest(new PageResult("title", content, new TestCounts()));
            Clock.Instance = new Clock();
            ClassicAssert.AreEqual(comment + expected, folder.GetPageContent(new FilePath(System.IO.Path.Combine("output", "myfile.html"))));
        }

        StoryTestFile MakeStoryTestFile(string fileName) {
            folder.MakeFile(fileName, "stuff");
            var memory = new TypeDictionary();
            memory.GetItem<Settings>().OutputFolder = "output";
            var file = new StoryTestFile(fileName,
                new StoryTestFolder(memory, folder, new Filters(string.Empty, new FileExclusions(), string.Empty)), folder);
            return file;
        }

        static readonly string comment = "<!--2016-01-02 13:14:15,0,0,0,0--><!-- saved from url=(0014)about:internet -->" + Environment.NewLine;
    }
}
