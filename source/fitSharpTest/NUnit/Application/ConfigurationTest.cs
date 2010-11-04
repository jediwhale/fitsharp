// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using System.Threading;
using System.Globalization;

namespace fitSharp.Test.NUnit.Application {
    [TestFixture] public class ConfigurationTest {

        private Configuration configuration;

        [SetUp] public void SetUp() {
            configuration = new Configuration();
        }

        [Test] public void MethodIsExecuted() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Application.TestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.NUnit.Application.TestConfig></config>");
            Assert.AreEqual("stuff", configuration.GetItem<TestConfig>().Data);
        }

        [Test] public void MethodWithTwoParametersIsExecuted() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Application.TestConfig><TestMethod second=\"more\">stuff</TestMethod></fitSharp.Test.NUnit.Application.TestConfig></config>");
            Assert.AreEqual("more stuff", configuration.GetItem<TestConfig>().Data);
        }

        [Test] public void TwoFilesAreLoadedIncrementally() {
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Application.TestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.NUnit.Application.TestConfig></config>");
            configuration.LoadXml("<config><fitSharp.Test.NUnit.Application.TestConfig><Append>more</Append></fitSharp.Test.NUnit.Application.TestConfig></config>");
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

        [Test] public void AliasTypeIsUsed() {
            configuration.LoadXml("<config><fit.Settings><inputFolder>stuff</inputFolder></fit.Settings></config>");
            Assert.AreEqual("stuff", configuration.GetItem<Settings>().InputFolder);
        }

        [Test] public void AliasMethodIsUsed() {
            configuration.LoadXml("<config><fit.Namespaces><add>fitSharp.Test.NUnit.Application</add></fit.Namespaces></config>");
            Assert.IsNotNull(configuration.GetItem<ApplicationUnderTest>().FindType(new IdentifierName("ConfigurationTest")));
        }

        [Test] public void DefaultCultureIfNoneIsSpecified()
        {
          const string configWithoutCulture = "<config><fit.Settings/></config>";
          configuration.LoadXml(configWithoutCulture);
          Assert.AreEqual(Settings.DefaultCulture, configuration.GetItem<Settings>().CultureInfo);
        }

        [Test] public void DefaultCultureIfEmptyIsSpecified()
        {
          const string configWithEmptyCulture = "<config><fit.Settings><culture/></fit.Settings></config>";
          configuration.LoadXml(configWithEmptyCulture);
          Assert.AreEqual(Settings.DefaultCulture, configuration.GetItem<Settings>().CultureInfo);
        }

        [Test] public void DefaultCultureIfInvalidIsSpecified()
        {
          const string configWithInvalidCulture = "<config><fit.Settings><culture>xx-xx</culture></fit.Settings></config>";
          configuration.LoadXml(configWithInvalidCulture);
          Assert.AreEqual(Settings.DefaultCulture, configuration.GetItem<Settings>().CultureInfo);
        }
      
        [Test] public void ValidCultureIsAccepted()
        {
          const string configWithValidCulture = "<config><fit.Settings><culture>af-ZA</culture></fit.Settings></config>";
          configuration.LoadXml(configWithValidCulture);
          Assert.AreEqual(CultureInfo.GetCultureInfo("af-ZA"), configuration.GetItem<Settings>().CultureInfo);
        }

        [Test] public void InvariantCultureIsAccepted()
        {
          configuration.LoadXml("<config><fit.Settings><culture>invariant</culture></fit.Settings></config>");
          Assert.AreEqual(CultureInfo.InvariantCulture, configuration.GetItem<Settings>().CultureInfo);

          configuration.LoadXml("<config><fit.Settings><culture>iNVarIaNt</culture></fit.Settings></config>");
          Assert.AreEqual(CultureInfo.InvariantCulture, configuration.GetItem<Settings>().CultureInfo);        
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