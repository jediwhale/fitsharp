// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using NUnit.Framework;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Application;
using fit.Runner;
using fitSharp.IO;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture] public class FolderRunnerTest {
        Memory memory;

        [SetUp] public void SetUp() {
            memory = new TypeDictionary();
            memory.GetItem<Settings>().DryRun = false;
            memory.GetItem<Settings>().InputFolder = "in";
            memory.GetItem<Settings>().OutputFolder = "out";
        }

        [Test] public void ParsingOfDryRunInputAndOutputFolderIsDoneCorrectly() {
            var runner = new FolderRunner();
            runner.Run(new[] { "-d", "-i", "overridden_in", "-o", "overridden_out" }, memory, new NullReporter());

            ClassicAssert.AreEqual(true, memory.GetItem<Settings>().DryRun);
            ClassicAssert.AreEqual("overridden_in", memory.GetItem<Settings>().InputFolder);
            ClassicAssert.AreEqual("overridden_out", memory.GetItem<Settings>().OutputFolder);
        }

        [Test] public void DryRunSuppressesSummaryReport() {
            memory.GetItem<Settings>().DryRun = true;

            var reporter = new CollectingReporter();

            var runner = new FolderRunner();
            runner.Run(new string[] {}, memory, reporter);

            ClassicAssert.AreEqual(string.Empty, reporter.Output);
        }
    }
}
