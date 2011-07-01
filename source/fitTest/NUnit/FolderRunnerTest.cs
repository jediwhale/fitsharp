// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using NUnit.Framework;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Application;
using fit.Runner;
using fitSharp.IO;

namespace fit.Test.NUnit {
    [TestFixture] public class FolderRunnerTest {
        Configuration configuration;

        [SetUp] public void SetUp() {
            configuration = new Configuration();
            configuration.GetItem<Settings>().DryRun = false;
            configuration.GetItem<Settings>().InputFolder = "in";
            configuration.GetItem<Settings>().OutputFolder = "out";
        }

        [Test] public void ParsingOfDryRunInputAndOutputFolderIsDoneCorrectly() {
            var runner = new FolderRunner();
            runner.Run(new[] { "-d", "-i", "overridden_in", "-o", "overridden_out" }, configuration, new NullReporter());

            Assert.AreEqual(true, configuration.GetItem<Settings>().DryRun);
            Assert.AreEqual("overridden_in", configuration.GetItem<Settings>().InputFolder);
            Assert.AreEqual("overridden_out", configuration.GetItem<Settings>().OutputFolder);
        }

        [Test] public void DryRunSuppressesSummaryReport() {
            configuration.GetItem<Settings>().DryRun = true;

            var reporter = new CollectingReporter();

            var runner = new FolderRunner();
            runner.Run(new string[] {}, configuration, reporter);

            Assert.AreEqual(string.Empty, reporter.Output);
        }
    }
}
