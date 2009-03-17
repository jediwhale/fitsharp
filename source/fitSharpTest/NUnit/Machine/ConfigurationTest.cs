// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ConfigurationTest {

        private Configuration configuration;

        [SetUp] public void SetUp() {
            configuration = new Configuration();
        }

        [Test] public void MethodIsExecuted() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.TestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.NUnit.Machine.TestConfig></config>");
            Assert.AreEqual("stuff", configuration.GetItem<TestConfig>().Data);
        }

        [Test] public void MethodWithTwoParametersIsExecuted() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.TestConfig><TestMethod second=\"more\">stuff</TestMethod></fitSharp.Test.NUnit.Machine.TestConfig></config>");
            Assert.AreEqual("more stuff", configuration.GetItem<TestConfig>().Data);
        }

        [Test] public void TwoFilesAreLoadedIncrementally() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.TestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.NUnit.Machine.TestConfig></config>");
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Machine.TestConfig><Append>more</Append></fitSharp.Test.NUnit.Machine.TestConfig></config>");
            Assert.AreEqual("stuffmore", configuration.GetItem<TestConfig>().Data);
        }

        [Test] public void ChangesDontShowInCopy() {
            var test = new TestConfig {Data = "stuff"};
            configuration.SetItem(test.GetType(), test);
            var copy = new Configuration(configuration);
            configuration.GetItem<TestConfig>().Data = "other";
            Assert.AreEqual("stuff", copy.GetItem<TestConfig>().Data);
            Assert.AreEqual("other", configuration.GetItem<TestConfig>().Data);
        }
    }

    public class TestConfig: Copyable {
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
            return new TestConfig {Data = Data};
        }
    }
}