// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fit.Test.Acceptance;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ActionFixtureTest
    {
        private Parse table;
        private Configuration configuration;

        [SetUp]
        public void SetUp()
        {
            configuration = TestUtils.InitAssembliesAndNamespaces();
        }

        private static string BuildTable(string name)
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
            return builder.ToString();
        }

        [Test]
        public void TestStart()
        {
            table = new Parse(BuildTable("ActionFixture"));
            var fixture = new ActionFixture { Processor = new Service.Service(configuration) };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.TestStatus.Counts.GetCount(CellAttributes.ExceptionStatus), table.ToString());
            Assert.IsNotNull(fixture.GetTargetObject());
        }

        [Test]
        public void TestCheck()
        {
            table = new Parse(BuildTable("ActionFixture"));
            var fixture = new ActionFixture{ Processor = new Service.Service(configuration) };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.TestStatus.Counts.GetCount(CellAttributes.ExceptionStatus), table.ToString());
            var countFixture = (CountFixture)fixture.GetTargetObject();
            int actualCount = countFixture.Counter;
            Assert.AreEqual(6, actualCount);
            Assert.AreEqual(4, fixture.TestStatus.Counts.GetCount(CellAttributes.RightStatus));
        }

        [Test]
        public void TestCheckOnTimedActionFixture()
        {
            table = new Parse(BuildTable("TimedActionFixture"));
            var fixture = new ActionFixture{ Processor = new Service.Service(configuration) };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.TestStatus.Counts.GetCount(CellAttributes.ExceptionStatus), table.ToString());
            var countFixture = (CountFixture)fixture.GetTargetObject();
            int actualCount = countFixture.Counter;
            Assert.AreEqual(6, actualCount);
            Assert.AreEqual(4, countFixture.TestStatus.Counts.GetCount(CellAttributes.RightStatus));
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

        private class MyActionFixture: ActionFixture {
            public MyActionFixture(Fixture actor, Parse cells) {
                ActionFixture.actor = actor;
                this.cells = cells;
            }
        }
    }
}