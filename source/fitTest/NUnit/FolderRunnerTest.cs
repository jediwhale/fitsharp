using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Application;
using fit.Runner;
using fitSharp.IO;

namespace fit.Test.NUnit
{
    [TestFixture]
    public class FolderRunnerTest
    {
        Configuration configuration;

        [SetUp]
        public void SetUp()
        {
            configuration = new Configuration();
            configuration.GetItem<Settings>().DryRun = false;
            configuration.GetItem<Settings>().InputFolder = "in";
            configuration.GetItem<Settings>().OutputFolder = "out";
        }

        [Test]
        public void ParsingOfDryRunInputAndOutputFolderIsDoneCorrectly() {
            FolderRunner runner = new FolderRunner();
            runner.Run(new string[] { "-d", "-i", "overridden_in", "-o", "overridden_out" }, configuration, new NullReporter());

            Assert.AreEqual(true, configuration.GetItem<Settings>().DryRun);
            Assert.AreEqual("overridden_in", configuration.GetItem<Settings>().InputFolder);
            Assert.AreEqual("overridden_out", configuration.GetItem<Settings>().OutputFolder);
        }

        [Test]
        public void DryRunSuppressesSummaryReport() {
            configuration.GetItem<Settings>().DryRun = true;

            var reporter = new CollectingReporter();

            FolderRunner runner = new FolderRunner();
            runner.Run(new string[] {}, configuration, reporter);

            Assert.AreEqual(string.Empty, reporter.Output);
        }
    }
}
