// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Runner;
using fitSharp.Fit.Runner;
using fitSharp.IO;
using fitSharp.Machine.Application;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class SuiteRunnerTest {

        [Test] public void ExecutesSuiteTearDownLast() {
            var folders = new FolderTestModel();
            folders.MakeFile("in\\suiteteardown.html", "<table><tr><td>fixture</td></tr></table>");
            folders.MakeFile("in\\zzzz.html", "<table><tr><td>fixture</td></tr></table>");
            var config = new Configuration();
            config.GetItem<Settings>().InputFolder = "in";
            config.GetItem<Settings>().OutputFolder = "out";
            var runner = new SuiteRunner(config, new NullReporter());
            runner.Run(new StoryTestFolder(config, folders), string.Empty);
            int tearDown = folders.FileContent("out\\reportIndex.html").IndexOf("suiteteardown.html");
            int otherFile = folders.FileContent("out\\reportIndex.html").IndexOf("zzzz.html");
            Assert.IsTrue(otherFile < tearDown);
        }
    }
}
