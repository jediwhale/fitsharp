// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Runner;
using fitSharp.Fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Test.Double;
using NUnit.Framework;
using System;

namespace fit.Test.NUnit {
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
            AddTestFile(@"in\suiteteardown.html");
            AddTestFile(@"in\zzzz.html");

            RunSuite();

            int tearDown = folders.FileContent(@"out\reportIndex.html").IndexOf("suiteteardown.html");
            int otherFile = folders.FileContent(@"out\reportIndex.html").IndexOf("zzzz.html");
            Assert.IsTrue(otherFile < tearDown);
        }

        [Test] public void SuiteSetupAndTearDownAreIncludedInDryRun() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile(@"in\suitesetup.html");
            AddTestFile(@"in\test.html");
            AddTestFile(@"in\suiteteardown.html");

            var reporter = new CollectingReporter();
            RunSuite(reporter);

            string expected = @"in\suitesetup.html" + Environment.NewLine +
                              @"in\test.html" + Environment.NewLine +
                              @"in\suiteteardown.html" + Environment.NewLine;
            Assert.AreEqual(expected, reporter.Output);
        }

        [Test] public void SetupAndTearDownAreNotIncludedInDryRun() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile(@"in\setup.html");
            AddTestFile(@"in\test.html");
            AddTestFile(@"in\teardown.html");

            var reporter = new CollectingReporter();
            RunSuite(reporter);

            string expected = @"in\test.html" + Environment.NewLine;
                              
            Assert.AreEqual(expected, reporter.Output);
        }

        [Test] public void DryRunListsAllApplicableTestFiles() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile(@"in\test1.html");
            AddTestFile(@"in\test2.html");

            var reporter = new CollectingReporter();
            RunSuite(reporter);

            string expected = @"in\test1.html" + Environment.NewLine +
                              @"in\test2.html" + Environment.NewLine;
            Assert.AreEqual(expected, reporter.Output);
        }

        [Test] public void DryRunDoesNotProduceAnyTestResults() {
            memory.GetItem<Settings>().DryRun = true;

            AddTestFile(@"in\test1.html");
            AddTestFile(@"in\test2.html");

            RunSuite();

            Assert.IsEmpty(folders.GetFiles("out"));
            Assert.IsEmpty(folders.GetFolders("out"));
        }

        [Test]
        public void TestPagePathIsStoredInMemoryWhenPageExecutes() {
            Assert.IsNullOrEmpty(memory.GetItem<Context>().TestPagePath);

            AddTestFile(@"in\page.html");
            RunSuite();

            Assert.AreEqual(@"in\page.html", memory.GetItem<Context>().TestPagePath);
        }

        [Test]
        public void SetupAndTearDownAreNotCopiedToOutputDirectory() {
            AddTestFile(@"in\setup.html");
            AddTestFile(@"in\test.html");
            AddTestFile(@"in\teardown.html");

            RunSuite();

            Assert.IsTrue(folders.FileExists(@"out\test.html"), "test.html should exist in output directory");

            Assert.IsFalse(folders.FileExists(@"out\setup.html"), "setup.html should not exist in output directory");
            Assert.IsFalse(folders.FileExists(@"out\teardown.html"), "teardown.html should not exist in output directory");
        }

        private void RunSuite() {
            RunSuite(new NullReporter());
        }

        private void RunSuite(ProgressReporter reporter) {
            var runner = new SuiteRunner(memory, reporter);
            runner.Run(new StoryTestFolder(memory, folders), string.Empty);
        }

        private void AddTestFile(string path) {
            folders.MakeFile(path, "<table><tr><td>fixture</td></tr></table>");
        }
    }
}
