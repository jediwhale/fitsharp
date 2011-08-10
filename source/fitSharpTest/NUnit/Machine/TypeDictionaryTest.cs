// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class TypeDictionaryTest {

        private TypeDictionary configuration;

        [SetUp] public void SetUp() {
            configuration = new TypeDictionary();
        }

        [Test] public void CopyableChangesDontShowInCopy() {
            configuration.GetItem<FullTestConfig>().Data = "stuff";
            var copy = configuration.Copy();
            configuration.GetItem<FullTestConfig>().Data = "other";
            Assert.AreEqual("stuff", copy.GetItem<FullTestConfig>().Data);
            Assert.AreEqual("other", configuration.GetItem<FullTestConfig>().Data);
        }

        [Test] public void OtherChangesShowInCopy() {
            configuration.GetItem<SimpleTestConfig>().Data = "stuff";
            var copy = configuration.Copy();
            configuration.GetItem<SimpleTestConfig>().Data = "other";
            Assert.AreEqual("other", copy.GetItem<SimpleTestConfig>().Data);
            Assert.AreEqual("other", configuration.GetItem<SimpleTestConfig>().Data);
        }


        [Test] public void SetUpTearDownIsExecuted() {
            configuration.GetItem<FullTestConfig>().Data = "stuff";
            configuration.Apply(i => i.As<SetUpTearDown>(s => s.SetUp()));
            Assert.AreEqual("setup", configuration.GetItem<FullTestConfig>().Data);
            configuration.Apply(i => i.As<SetUpTearDown>(s => s.TearDown()));
            Assert.AreEqual("teardown", configuration.GetItem<FullTestConfig>().Data);
        }
    }

    public class SimpleTestConfig {
        public string Data;
    }
}