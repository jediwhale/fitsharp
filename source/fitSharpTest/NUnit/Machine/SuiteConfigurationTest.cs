﻿// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class SuiteConfigurationTest {
        [SetUp] public void SetUp() {
            memory = new TypeDictionary();
            suiteConfiguration = new SuiteConfiguration(memory);
        }
        [Test] public void MethodIsExecuted() {
            suiteConfiguration.LoadXml("<config><fitSharp.Test.Double.FullTestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.Double.FullTestConfig></config>");
            ClassicAssert.AreEqual("stuff", memory.GetItem<FullTestConfig>().Data);
        }

        [Test] public void MethodWithTwoParametersIsExecuted() {
            suiteConfiguration.LoadXml("<config><fitSharp.Test.Double.FullTestConfig><TestMethod second=\"more\">stuff</TestMethod></fitSharp.Test.Double.FullTestConfig></config>");
            ClassicAssert.AreEqual("more stuff", memory.GetItem<FullTestConfig>().Data);
        }

        [Test] public void TwoFilesAreLoadedIncrementally() {
            suiteConfiguration.LoadXml("<config><fitSharp.Test.Double.FullTestConfig><TestMethod>stuff</TestMethod></fitSharp.Test.Double.FullTestConfig></config>");
            suiteConfiguration.LoadXml("<config><fitSharp.Test.Double.FullTestConfig><Append>more</Append></fitSharp.Test.Double.FullTestConfig></config>");
            ClassicAssert.AreEqual("stuffmore", memory.GetItem<FullTestConfig>().Data);
        }

        [Test] public void AliasTypeIsUsed() {
            suiteConfiguration.LoadXml("<config><fit.Settings><inputFolder>stuff</inputFolder></fit.Settings></config>");
            ClassicAssert.AreEqual("stuff", memory.GetItem<Settings>().InputFolder);
        }

        [Test] public void AliasMethodIsUsed() {
            suiteConfiguration.LoadXml("<config><fit.Namespaces><add>fitSharp.Test.NUnit.Machine</add></fit.Namespaces></config>");
            ClassicAssert.IsNotNull(memory.GetItem<ApplicationUnderTest>().FindType(new IdentifierName("SuiteConfigurationTest")));
        }

        Memory memory;
        SuiteConfiguration suiteConfiguration;
    }
}
