// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ConfigurationTest {

        private Configuration configuration;

        [SetUp] public void SetUp() {
            configuration = new Configuration();
        }

        [Test] public void MethodIsExecuted() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.FullTestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.NUnit.Machine.FullTestConfig></config>");
            Assert.AreEqual("stuff", configuration.GetItem<FullTestConfig>().Data);
        }

        [Test] public void MethodWithTwoParametersIsExecuted() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.FullTestConfig><TestMethod second=\"more\">stuff</TestMethod></fitSharp.Test.NUnit.Machine.FullTestConfig></config>");
            Assert.AreEqual("more stuff", configuration.GetItem<FullTestConfig>().Data);
        }

        [Test] public void TwoFilesAreLoadedIncrementally() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.FullTestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.NUnit.Machine.FullTestConfig></config>");
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.FullTestConfig><Append>more</Append></fitSharp.Test.NUnit.Machine.FullTestConfig></config>");
            Assert.AreEqual("stuffmore", configuration.GetItem<FullTestConfig>().Data);
        }

        [Test] public void CopyableChangesDontShowInCopy() {
            var test = new FullTestConfig {Data = "stuff"};
            configuration.SetItem(test.GetType(), test);
            var copy = new Configuration(configuration);
            configuration.GetItem<FullTestConfig>().Data = "other";
            Assert.AreEqual("stuff", copy.GetItem<FullTestConfig>().Data);
            Assert.AreEqual("other", configuration.GetItem<FullTestConfig>().Data);
        }

        [Test] public void OtherChangesShowInCopy() {
            var test = new SimpleTestConfig {Data = "stuff"};
            configuration.SetItem(test.GetType(), test);
            var copy = new Configuration(configuration);
            configuration.GetItem<SimpleTestConfig>().Data = "other";
            Assert.AreEqual("other", copy.GetItem<SimpleTestConfig>().Data);
            Assert.AreEqual("other", configuration.GetItem<SimpleTestConfig>().Data);
        }

        [Test] public void AliasTypeIsUsed() {
            configuration.LoadXml("<config><fit.Settings><inputFolder>stuff</inputFolder></fit.Settings></config>");
            Assert.AreEqual("stuff", configuration.GetItem<Settings>().InputFolder);
        }

        [Test] public void AliasMethodIsUsed() {
            configuration.LoadXml("<config><fit.Namespaces><add>fitSharp.Test.NUnit.Machine</add></fit.Namespaces></config>");
            Assert.IsNotNull(configuration.GetItem<ApplicationUnderTest>().FindType(new IdentifierName("ConfigurationTest")));
        }

        [Test] public void SetUpTearDownIsExecuted() {
            configuration.SetItem(typeof(FullTestConfig), new FullTestConfig());
            configuration.SetUp();
            Assert.AreEqual("setup", configuration.GetItem<FullTestConfig>().Data);
            configuration.TearDown();
            Assert.AreEqual("teardown", configuration.GetItem<FullTestConfig>().Data);
        }
    }

    public class FullTestConfig: Copyable, SetUpTearDown {
        public string Data;

        public void TestMethod(string data) {
            Data = data;
        }

        public void TestMethod(string data, string more) {
            Data = more + " " + data;
        }

        public void Append(string data) {
            Data += data;
        }

        public Copyable Copy() {
            return new FullTestConfig {Data = Data};
        }

        public void SetUp() {
            Data = "setup";
        }

        public void TearDown() {
            Data = "teardown";
        }
    }

    public class SimpleTestConfig {
        public string Data;
    }
}