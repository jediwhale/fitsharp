// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ColumnFixtureTests
    {
        TestCounts resultCounts;

        [Test]
        public void TestNullCell()
        {
            var builder = new StringBuilder();
            //builder.Append("<table>");
            //builder.Append("<tr><td>configuration setup</td></tr>");
            //builder.Append("<tr><td>service</td></tr>");
            //builder.Append("<tr><td>add operator</td><td>fit.operators.executeempty</td></tr>");
            //builder.Append("</table>");
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">string fixture</td></tr>");
            builder.Append("<tr><td>Field</td><td>Field?</td></tr>");
            builder.Append("<tr><td></td><td>null</td></tr>");
            builder.Append("</table>");

            var parse = Parse.ParseFrom(builder.ToString());

            TestUtils.InitAssembliesAndNamespaces();
            RunTest(parse);
            TestUtils.CheckCounts(resultCounts, 1, 0, 0, 0);
        }

        void RunTest(Parse parse) {
            var test = new StoryTest(parse, (t,c) => { resultCounts = c;});
            test.Execute(new Configuration());
        }

        [Test]
        public void TestBlankCell()
        {
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"6\">string fixture</td></tr>");
            builder.Append("<tr><td>field</td><td>field?</td><td>property</td><td>property?</td><td>set</td><td>get?</td></tr>");
            builder.Append("<tr><td>blank</td><td>blank</td><td>blank</td><td>blank</td><td>blank</td><td>blank</td></tr>");
            builder.Append("</table>");

            var parse = Parse.ParseFrom(builder.ToString());

            TestUtils.InitAssembliesAndNamespaces();
            RunTest(parse);
            TestUtils.CheckCounts(resultCounts, 3, 0, 0, 0);
        }

        [Test]
        public void TestExecuteAtEnd()
        {
            TestUtils.InitAssembliesAndNamespaces();
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">ExecuteTestFixture</td></tr>");
            builder.Append("<tr><td>Property</td><td>Property</td></tr>");
            builder.Append("<tr><td>first call</td><td>second call</td></tr>");
            builder.Append("</table>");
            var table = Parse.ParseFrom(builder.ToString());
            var testFixture = new ExecuteTestFixture { Processor = new Service.Service()};
            testFixture.DoTable(table);
            Assert.AreEqual(3, testFixture.Values.Count);
            Assert.AreEqual("first call", testFixture.Values[0]);
            Assert.AreEqual("second call", testFixture.Values[1]);
            Assert.AreEqual("Execute()", testFixture.Values[2]);
        }

        [Test]
        public void TestExecuteInMiddle()
        {
            TestUtils.InitAssembliesAndNamespaces();
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">ExecuteTestFixture</td></tr>");
            builder.Append("<tr><td>Property</td><td>Property?</td><td>Property</td></tr>");
            builder.Append("<tr><td>first call</td><td>null</td><td>second call</td></tr>");
            builder.Append("</table>");
            var table = Parse.ParseFrom(builder.ToString());
            var testFixture = new ExecuteTestFixture { Processor = new Service.Service()};
            testFixture.DoTable(table);
            Assert.AreEqual(3, testFixture.Values.Count);
            Assert.AreEqual("first call", testFixture.Values[0]);
            Assert.AreEqual("Execute()", testFixture.Values[1]);
            Assert.AreEqual("second call", testFixture.Values[2]);
        }

        [Test]
        public void TestExecuteWithMethod()
        {
            TestUtils.InitAssembliesAndNamespaces();
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">ExecuteTestFixture</td></tr>");
            builder.Append("<tr><td>Property</td><td>BoolMethod?</td></tr>");
            builder.Append("<tr><td>first call</td><td>true</td></tr>");
            builder.Append("</table>");
            var table = Parse.ParseFrom(builder.ToString());
            var testFixture = new ExecuteTestFixture { Processor = new Service.Service()};
            testFixture.DoTable(table);
            Assert.AreEqual(3, testFixture.Values.Count);
            Assert.AreEqual("first call", testFixture.Values[0]);
            Assert.AreEqual("Execute()", testFixture.Values[1]);
            Assert.AreEqual("method!", testFixture.Values[2]);
        }

        [Test]
        public void TestGetTargetObject() {
            Fixture fixture = new ExecuteTestFixture();
            Assert.AreEqual(fixture, fixture.GetTargetObject());
        }

        [Test]
        public void TestEmptyHeaderCell()
        {
            TestUtils.InitAssembliesAndNamespaces();
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">string fixture</td></tr>");
            builder.Append("<tr><td>field</td><td></td></tr>");
            builder.Append("<tr><td>some value</td><td>this is a comment</td></tr>");
            builder.Append("</table>");
            var table = Parse.ParseFrom(builder.ToString());
            RunTest(table);
            TestUtils.CheckCounts(resultCounts, 0, 0, 0, 0);
        }

        [Test]
        public void TestExecuteDoesNotCauseMethodsToGetCalledThrice()
        {
            TestUtils.InitAssembliesAndNamespaces();
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">ExecuteTestFixture</td></tr>");
            builder.Append("<tr><td>Method()</td></tr>");
            builder.Append("<tr><td>1</td></tr>");
            builder.Append("<tr><td>2</td></tr>");
            builder.Append("</table>");
            var table = Parse.ParseFrom(builder.ToString());
            var testFixture = new ExecuteTestFixture { Processor = new Service.Service()};
            testFixture.DoTable(table);
            Assert.AreEqual(4, testFixture.Values.Count);
            Assert.AreEqual("Execute()", testFixture.Values[0]);
            Assert.AreEqual("Method()", testFixture.Values[1]);
            Assert.AreEqual("Execute()", testFixture.Values[2]);
            Assert.AreEqual("Method()", testFixture.Values[3]);
            TestUtils.VerifyCounts(testFixture, 2, 0, 0, 0);
        }
    }

    public class ExecuteTestFixture : ColumnFixture
    {
        public IList Values = new ArrayList();

        public bool BoolMethod()
        {
            Values.Add("method!");
            return true;
        }

        public string Property
        {
            get { return null; }
            set { Values.Add(value); }
        }

        int callsToMethod = 1;

        public int Method()
        {
            Values.Add("Method()");
            return callsToMethod++;
        }

        public override void Execute()
        {
            Values.Add("Execute()");
        }
    }
}
