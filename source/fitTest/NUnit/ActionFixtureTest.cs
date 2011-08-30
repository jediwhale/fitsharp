// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fit.Test.Acceptance;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ActionFixtureTest
    {
        Parse table;
        Memory memory;

        [SetUp]
        public void SetUp()
        {
            memory = TestUtils.InitAssembliesAndNamespaces();
        }

        static Parse BuildTable(string name)
        {
            var builder = new StringBuilder();
            builder.Append("<table border=\"1\" cellspacing=\"0\">");
            builder.Append("<tr><td colspan=\"3\">").Append(name).Append("</td></tr>");
            builder.Append("<tr><td>start</td><td colspan=\"2\">Count Fixture</td></tr>");
            builder.Append("<tr><td>check</td><td>Counter</td><td>0</td></tr>");
	
            builder.Append("<tr><td>press</td><td colspan=\"2\">Count</td></tr>");
            builder.Append("<tr><td>check</td><td>Counter</td><td>1</td></tr>");
            builder.Append("<tr><td>press</td><td colspan=\"2\">Count</td></tr>");
            builder.Append("<tr><td>check</td><td>Counter</td><td>2</td></tr>");
            builder.Append("<tr><td>enter</td><td>Counter</td><td>5</td></tr>");
	
            builder.Append("<tr><td>press</td><td colspan=\"2\">Count</td></tr>");
            builder.Append("<tr><td>check</td><td>Counter</td><td>6</td></tr>");
            builder.Append("</table>");
            return Parse.ParseFrom(builder.ToString());
        }

        [Test]
        public void TestStart()
        {
            table = BuildTable("ActionFixture");
            var fixture = new ActionFixture { Processor = new Service.Service(memory) };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.TestStatus.Counts.GetCount(TestStatus.Exception), table.ToString());
            Assert.IsNotNull(fixture.GetTargetObject());
        }

        [Test]
        public void TestCheck()
        {
            table = BuildTable("ActionFixture");
            var fixture = new ActionFixture{ Processor = new Service.Service(memory) };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.TestStatus.Counts.GetCount(TestStatus.Exception), table.ToString());
            var countFixture = (CountFixture)fixture.GetTargetObject();
            int actualCount = countFixture.Counter;
            Assert.AreEqual(6, actualCount);
            Assert.AreEqual(4, fixture.TestStatus.Counts.GetCount(TestStatus.Right));
        }

        [Test]
        public void TestCheckOnTimedActionFixture()
        {
            table = BuildTable("TimedActionFixture");
            var fixture = new ActionFixture{ Processor = new Service.Service(memory) };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.TestStatus.Counts.GetCount(TestStatus.Exception), table.ToString());
            var countFixture = (CountFixture)fixture.GetTargetObject();
            int actualCount = countFixture.Counter;
            Assert.AreEqual(6, actualCount);
            Assert.AreEqual(4, countFixture.TestStatus.Counts.GetCount(TestStatus.Right));
        }

        [Test] public void PressInvokesMethodOnActor() {
            var method = new Parse("td", "method", null, null);
            var cells = new Parse("td", "press", null, method);
            var cellOperation = new Mock<CellOperation>();
            var actor = new Fixture();
            var actionFixture = new MyActionFixture(actor, cells) {CellOperation = cellOperation.Object};
            actionFixture.Press();
            cellOperation.Verify(o => o.TryInvoke(actor, method, It.Is<Tree<Cell>>(t => t.IsLeaf), method));
        }

        class MyActionFixture: ActionFixture {
            public MyActionFixture(Fixture actor, Parse cells) {
                ActionFixture.actor = actor;
                this.cells = cells;
            }
        }
    }
}
