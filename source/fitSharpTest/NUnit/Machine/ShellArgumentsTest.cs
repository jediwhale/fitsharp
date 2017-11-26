// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture]
    public class ShellArgumentsTest {

        [Test]
        public void SpecifiesRunner() {
            Parse(AssertRunnerSpecified, AssertNothingReported, "-r", "myRunner");
        }

        [Test]
        public void SpecifiesAppConfigFile() {
            folderModel.MakeFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "myConfig.xml"), "stuff");
            Parse(AssertAppConfigSpecified, AssertNothingReported, "-a", "myConfig.xml", "-r", "myRunner");
        }

        [Test]
        public void LoadsSuiteConfigFile() {
            folderModel.MakeFile("myConfig.xml", configContent);
            Parse(AssertRunnerSpecified, AssertNothingReported, "-c", "myConfig.xml");
        }

        [Test]
        public void ChecksForMissingRunner() {
            Parse(AssertNoAction, AssertRunnerReported);
        }

        [Test]
        public void ChecksForMissingAppConfigFile() {
            Parse(AssertNoAction, AssertAppConfigReported, "-a", "missing.xml");
        }

        [Test]
        public void ChecksForMissingSuiteConfigFile() {
            Parse(AssertNoAction, AssertSuiteConfigReported, "-c", "missing.xml");
        }

        void Parse(Func<Memory, int> process, Action<Error> report, params string[] commandLineArguments) {
            var arguments = new ShellArguments(folderModel, commandLineArguments);
            arguments.LoadMemory().Select(
                e => {
                    report(e);
                    return 1;
                },
                process);
        }

        static int AssertRunnerSpecified(Memory memory) {
            Assert.AreEqual("myRunner", memory.GetItem<Settings>().Runner);
            return 0;
        }

        static int AssertAppConfigSpecified(Memory memory) {
            memory.GetItem<AppDomainSetup>().ApplicationBase = ".";
            Assert.IsTrue(memory.GetItem<AppDomainSetup>().ConfigurationFile.EndsWith("myConfig.xml"));
            return 0;
        }

        static int AssertNoAction(Memory memory) {
            Assert.Fail();
            return 0;
        }

        static void AssertNothingReported(Error error) { Assert.Fail(error.Message); }
        static void AssertRunnerReported(Error error) { AssertReportContains(error, "runner"); }
        static void AssertAppConfigReported(Error error) { AssertReportContains(error, "Application configuration file"); }
        static void AssertSuiteConfigReported(Error error) { AssertReportContains(error, "Suite configuration file"); }

        static void AssertReportContains(Error error, string expected) {
            Assert.IsTrue(error.Message.Contains(expected), error.Message);
        }

        [SetUp]
        public void SetUp() {
            folderModel = new FolderTestModel();
        }

        FolderTestModel folderModel;

        const string configContent = "<?xml version=\"1.0\" ?><suiteConfig><Settings><Runner>myRunner</Runner></Settings></suiteConfig>";
    }
}
