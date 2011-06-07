// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Fixtures;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ConfigureFixtureTest {
        CellProcessorBase processor;
        ConfigureFixture fixture;

        [SetUp] public void SetUp() {
            processor = new CellProcessorBase();
            fixture = new ConfigureFixture();
            fixture.Prepare(processor, null, new CellTree());
        }

        [Test] public void InvokesMethodOnConfigurationItem() {
            fixture.Interpret(MakeTable("start"));
            var item = processor.Configuration.GetItem<Logging>();
            item.WriteItem("stuff");
            Assert.AreEqual("<ul><li>stuff</li></ul>", item.Show);
        }

        static CellTree MakeTable(string method) {
            return new CellTree(new CellTree("configure", "logging", method));
        }

        [Test] public void DisplaysResultInTable() {
            var item = processor.Configuration.GetItem<Logging>();
            item.Start();
            item.WriteItem("stuff");
            var table = MakeTable("show");
            fixture.Interpret(table);
            Assert.AreEqual("<ul><li>stuff</li></ul>", table.Branches[0].Branches[2].Value.GetAttribute(CellAttribute.Folded));
        }
    }
}
