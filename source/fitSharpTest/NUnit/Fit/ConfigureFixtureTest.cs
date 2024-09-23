// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ConfigureFixtureTest {
        CellProcessor processor;
        ConfigureFixture fixture;

        [SetUp] public void SetUp() {
            processor = Builder.CellProcessor();
            fixture = new ConfigureFixture();
        }

        [Test] public void InvokesMethodOnConfigurationItem() {
            fixture.Interpret(processor, MakeTable("logging", "start"));
            var item = processor.Configuration.GetItem<Logging>();
            item.WriteItem("stuff");
            ClassicAssert.AreEqual("<ul><li>stuff</li></ul>", item.Show);
        }

        [Test] public void InvokesMethodWithParameters() {
            fixture.Interpret(processor, new CellTree(new CellTree("configure", "symbols", "save value of System.String", "mysymbol", "", "myvalue")));
            ClassicAssert.AreEqual("myvalue", processor.Memory.GetItem<Symbols>().GetValue("mysymbol"));
        }

        static CellTree MakeTable(string facility, string method) {
            return new CellTree(new CellTree("configure", facility, method));
        }

        [Test] public void DisplaysResultInTable() {
            processor.Memory.GetItem<Symbols>().Save("mysymbol", "myvalue");
            var table = new CellTree(new CellTree("configure", "symbols", "getvalue", "mysymbol"));
            fixture.Interpret(processor, table);
            ClassicAssert.AreEqual("myvalue", table.ValueAt(0, 2).GetAttribute(CellAttribute.Folded));
        }

        [Test] public void InvokesMethodOnProcessor() {
            var table = MakeTable("processor", "teststatus");
            fixture.Interpret(processor, table);
            ClassicAssert.AreEqual("fitSharp.Fit.Model.TestStatus", table.ValueAt(0, 2).GetAttribute(CellAttribute.Folded));
        }

        [Test] public void InvokesMethodInEachRow() {
            var table = new CellTree(new CellTree("configure", "processor"), new CellTree("teststatus"));
            fixture.Interpret(processor, table);
            ClassicAssert.AreEqual("fitSharp.Fit.Model.TestStatus", table.ValueAt(1, 0).GetAttribute(CellAttribute.Folded));
        }
    }
}
