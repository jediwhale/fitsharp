// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ConfigurationTest {

        private Configuration configuration;

        [SetUp] public void SetUp() {
            configuration = new Configuration();
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


        [Test] public void SetUpTearDownIsExecuted() {
            configuration.SetItem(typeof(FullTestConfig), new FullTestConfig());
            configuration.SetUp();
            Assert.AreEqual("setup", configuration.GetItem<FullTestConfig>().Data);
            configuration.TearDown();
            Assert.AreEqual("teardown", configuration.GetItem<FullTestConfig>().Data);
        }
    }

    public class SimpleTestConfig {
        public string Data;
    }
}