// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Application;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Runner;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Samples;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class SuiteRunnerTest {

        Memory memory;
        FolderTestModel folders;

        [SetUp] public void SetUp() {
            memory = new TypeDictionary();
            memory.GetItem<Settings>().InputFolder = "in";
            memory.GetItem<Settings>().OutputFolder = "out";

            folders = new FolderTestModel();
        }

        [Test] public void ExecutesSuiteTearDownLast() {
            AddTestFile("in", "suiteteardown.html");
            AddTestFile("in", "zzzz.html");

            RunSuite();

            var report = System.IO.Path.Combine("out", "reportIndex.html");
            int tearDown = folders.GetPageContent(report).IndexOf("suiteteardown.html", StringComparison.Ordinal);
            int otherFile = folders.GetPageContent(report).IndexOf("zzzz.html", StringComparison.Ordinal);
            ClassicAssert.IsTrue(otherFile < tearDown);
        }

        [Test] public void SuiteSetupAndTearDownAreIncludedInDryRun() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile("in", "suitesetup.html");
            AddTestFile("in", "test.html");
            AddTestFile("in", "suiteteardown.html");

            var reporter = new CollectingReporter();
            RunSuite(reporter);

            string expected = System.IO.Path.Combine("in", "suitesetup.html") + Environment.NewLine +
                              System.IO.Path.Combine("in", "test.html") + Environment.NewLine +
                              System.IO.Path.Combine("in", "suiteteardown.html") + Environment.NewLine;
            ClassicAssert.AreEqual(expected, reporter.Output);
        }

        [Test] public void SetupAndTearDownAreNotIncludedInDryRun() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile("in", "setup.html");
            AddTestFile("in", "test.html");
            AddTestFile("in", "teardown.html");

            var reporter = new CollectingReporter();
            RunSuite(reporter);

            string expected = System.IO.Path.Combine("in", "test.html") + Environment.NewLine;
                              
            ClassicAssert.AreEqual(expected, reporter.Output);
        }

        [Test] public void DryRunListsAllApplicableTestFiles() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile("in", "test1.html");
            AddTestFile("in", "test2.html");

            var reporter = new CollectingReporter();
            RunSuite(reporter);

            string expected = System.IO.Path.Combine("in", "test1.html") + Environment.NewLine +
                              System.IO.Path.Combine("in", "test2.html") + Environment.NewLine;
            ClassicAssert.AreEqual(expected, reporter.Output);
        }

        [Test] public void DryRunDoesNotProduceAnyTestResults() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile("in", "test1.html");
            AddTestFile("in", "test2.html");

            RunSuite();

            ClassicAssert.IsEmpty(folders.GetFiles("out"));
            ClassicAssert.IsEmpty(folders.GetFolders("out"));
        }

        [Test]
        public void TestPagePathIsStoredInMemoryWhenPageExecutes() {
            ClassicAssert.IsNull(memory.GetItem<Context>().TestPagePath);

            AddTestFile("in", "page.html");
            RunSuite();

            ClassicAssert.AreEqual(System.IO.Path.Combine("in", "page.html"), memory.GetItem<Context>().TestPagePath.ToString());
        }

        [Test]
        public void SetupAndTearDownAreNotCopiedToOutputDirectory() {
            AddTestFile("in", "setup.html");
            AddTestFile("in", "test.html");
            AddTestFile("in", "teardown.html");

            RunSuite();

            ClassicAssert.IsTrue(folders.Exists(System.IO.Path.Combine("out", "test.html")), "test.html should exist in output directory");

            ClassicAssert.IsFalse(folders.Exists(System.IO.Path.Combine("out", "setup.html")), "setup.html should not exist in output directory");
            ClassicAssert.IsFalse(folders.Exists(System.IO.Path.Combine("out", "teardown.html")), "teardown.html should not exist in output directory");
        }

        [Test]
        public void StylesheetIsCreatedInOutputDirectory() {
            AddTestFile("in", "test.html");

            RunSuite();

            ClassicAssert.IsTrue(folders.Exists(System.IO.Path.Combine("out", "fit.css")), "fit.css should exist in output directory");
        }

        [Test]
        public void TestInSubFolderIsRun() {
            AddTestFile(System.IO.Path.Combine("in", "sub"), "test.html");
            RunSuite();
            ClassicAssert.IsTrue(folders.Exists(System.IO.Path.Combine("out", "sub", "test.html")), "test.html should exist in output directory");
        }

        [Test]
        public void SelectedTestIsRun() {
            AddTestFile("in", "test1.html");
            AddTestFile("in", "test2.html");
            RunSuite("test2.html");
            ClassicAssert.IsFalse(folders.Exists(System.IO.Path.Combine("out", "test1.html")), "test1.html should not exist in output directory");
            ClassicAssert.IsTrue(folders.Exists(System.IO.Path.Combine("out", "test2.html")), "test2.html should exist in output directory");
        }

        [Test]
        public void SelectedTestInSubFolderIsRun() {
            AddTestFile("in", "test1.html");
            AddTestFile("in", "test2.html");
            AddTestFile(System.IO.Path.Combine("in", "some", "sub"), "test2.html");
            RunSuite(System.IO.Path.Combine("sub", "test2.html"));
            ClassicAssert.IsFalse(folders.Exists(System.IO.Path.Combine("out", "test1.html")), "test1.html should not exist in out directory");
            ClassicAssert.IsFalse(folders.Exists(System.IO.Path.Combine("out", "test2.html")), "test2.html should not exist in out directory");
            ClassicAssert.IsTrue(folders.Exists(System.IO.Path.Combine("out", "some", "sub", "test2.html")), "test2.html should exist in out\\some\\sub directory");
        }

        private void RunSuite(string selectedFile = "") {
            RunSuite(new NullReporter(), selectedFile);
        }

        private void RunSuite(ProgressReporter reporter, string selectedFile = "") {
            var runner = new SuiteRunner(memory, reporter, m => new CellProcessorBase(m, m.GetItem<CellOperators>()), folders);
            runner.Run(new StoryTestFolder(memory, folders, new Filters(string.Empty, new FileExclusions(), selectedFile)), selectedFile);
        }

        private void AddTestFile(string path, string file) {
            folders.MakeFile(System.IO.Path.Combine(path, file), "<table><tr><td>fixture</td></tr></table>");
        }
    }
}
