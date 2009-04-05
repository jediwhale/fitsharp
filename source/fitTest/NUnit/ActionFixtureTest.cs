// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fit.Test.Acceptance;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ActionFixtureTest
    {
        private Parse table;

        [SetUp]
        public void SetUp()
        {
            TestUtils.InitAssembliesAndNamespaces();
        }

        private string BuildTable(string name)
        {
            StringBuilder builder = new StringBuilder();
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
            ActionFixture fixture = new ActionFixture { Service = new Service.Service() };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.Counts.Exceptions, table.ToString());
            Assert.IsNotNull((CountFixture) fixture.GetTargetObject());
        }

        [Test]
        public void TestCheck()
        {
            table = new Parse(BuildTable("ActionFixture"));
            ActionFixture fixture = new ActionFixture{ Service = new Service.Service() };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.Counts.Exceptions, table.ToString());
            CountFixture countFixture = (CountFixture)fixture.GetTargetObject();
            int actualCount = ((CountFixture)countFixture).Counter;
            Assert.AreEqual(6, actualCount);
            Assert.AreEqual(4, fixture.Counts.Right);
        }

        [Test]
        public void TestCheckOnTimedActionFixture()
        {
            table = new Parse(BuildTable("TimedActionFixture"));
            ActionFixture fixture = new ActionFixture{ Service = new Service.Service() };
            fixture.DoTable(table);
            Assert.AreEqual(0, fixture.Counts.Exceptions, table.ToString());
            CountFixture countFixture = (CountFixture)fixture.GetTargetObject();
            int actualCount = ((CountFixture)countFixture).Counter;
            Assert.AreEqual(6, actualCount);
            Assert.AreEqual(4, countFixture.Counts.Right);
        }
    }
}