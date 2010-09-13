// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using fit.Test.Acceptance;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class RowFixtureTest
    {
        readonly string rowFixtureName = typeof (NewRowFixtureDerivative).Name;
        Parse table;
        StoryTest myStoryTest;
        TestCounts resultCounts;
        Configuration configuration;

        public void TestExpectBlankOrNullAllCorrect()
        {
            DoTable(
                BuildTable(new[] {"null", "blank", "joe"}),
                BuildObjectArray(new[] {null, "", "joe"}),
                3, 0, 0, 0
                );
            DoTable(
                BuildTable(new[] {"Null", "Blank"}),
                BuildObjectArray(new[] {null, ""}),
                2, 0, 0, 0
                );
            DoTable(
                BuildTable(new[] {"NULL", "BLANK"}),
                BuildObjectArray(new[] {null, ""}),
                2, 0, 0, 0
                );
        }

        public void TestExpectBlankOrNullSomeWrong()
        {
            TestUtils.InitAssembliesAndNamespaces();
            Parse atable = BuildTable(new[] {"blank", "null"});
            DoTable(
                atable,
                BuildObjectArray(new[] {"", "this is not null"}),
                1, 2, 0, 0
                );
        }

        static Parse BuildTable(IEnumerable<string> values)
        {
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>BusinessObjectRowFixture</td></tr>");
            builder.Append("<tr><td>GetFirstString</td></tr>");
            foreach (string value in values)
            {
                builder.Append("<tr><td>" + value + "</td></tr>");
            }
            builder.Append("</table>");
            return Parse.ParseFrom(builder.ToString());
        }

        static object[] BuildObjectArray(ICollection<string> values)
        {
            var objects = new object[values.Count];
            int count = 0;
            foreach (string value in values)
            {
                objects[count++] = new BusinessObject(new[] {value});
            }
            return objects;
        }

        public void DoTable(Parse tables, object[] businessObjects, int right, int wrong, int ignores, int exceptions)
        {
            BusinessObjectRowFixture.objects = businessObjects;
            RunTest(tables);

            TestUtils.CheckCounts(resultCounts, right, wrong, ignores, exceptions);
        }

        void RunTest(Parse parse) {
            var test = new StoryTest(parse, (t,c) => { resultCounts = c;});
            test.Execute(configuration);
        }

        [Test]
        public void TestSurplus()
        {
            TestUtils.InitAssembliesAndNamespaces();
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>BusinessObjectRowFixture</td></tr>");
            builder.Append("<tr><td>GetFirstString</td></tr>");
            builder.Append("<tr><td>number1</td></tr>");
            builder.Append("</table>");
            var parse = Parse.ParseFrom(builder.ToString());

            BusinessObjectRowFixture.objects = new object[]
                                               {
                                                   new BusinessObject(new[] {"number1"}),
                                                   new BusinessObject(new[] {"number2"}),
                                                   new BusinessObject(new[] {"number3"})
                                               };

            RunTest(parse);
            Assert.IsTrue(parse.ToString().IndexOf("number1") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number2") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number3") > 0);
            TestUtils.CheckCounts(resultCounts, 1, 2, 0, 0);
        }

        [Test]
        public void TestMissing()
        {
            TestUtils.InitAssembliesAndNamespaces();
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>BusinessObjectRowFixture</td></tr>");
            builder.Append("<tr><td>GetFirstString</td></tr>");
            builder.Append("<tr><td>number1</td></tr>");
            builder.Append("<tr><td>number2</td></tr>");
            builder.Append("<tr><td>number3</td></tr>");
            builder.Append("</table>");
            var parse = Parse.ParseFrom(builder.ToString());

            BusinessObjectRowFixture.objects = new object[]
                                               {
                                                   new BusinessObject(new[] {"number1"}),
                                               };

            RunTest(parse);
            Assert.IsTrue(parse.ToString().IndexOf("number1") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number2") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number3") > 0);
            TestUtils.CheckCounts(resultCounts, 1, 2, 0, 0);
        }

        [Test]
        public void TestStartsWithHandlerInSecondColumn()
        {
            new Service.Service(configuration).AddOperator(typeof(CompareStartsWith).FullName);
            var builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>people row fixture</td></tr>");
            builder.Append("<tr><td>first name</td><td>last name</td></tr>");
            builder.Append("<tr><td>Nigel</td><td>Tuf..</td></tr>");
            builder.Append("</table>");
            PeopleLoaderFixture.people.Clear();
            PeopleLoaderFixture.people.Add(new Person("Nigel", "Tufnel"));
            var tables = Parse.ParseFrom(builder.ToString());
            RunTest(tables);
            Assert.IsTrue(tables.ToString().IndexOf("Tuf..") > -1);
            Assert.IsFalse(tables.ToString().IndexOf("Tufnel") > -1);
            TestUtils.CheckCounts(resultCounts, 2, 0, 0, 0);
        }

        [SetUp]
        public void SetUp()
        {
            configuration = TestUtils.InitAssembliesAndNamespaces();
            table = Parse.ParseFrom("<table><tr><td>" + rowFixtureName + "</td></tr><tr><td>name</td></tr></table>");
            NewRowFixtureDerivative.QueryValues.Clear();
        }

        [Test]
        public void TestZeroExpectedZeroActual() {
            RunTest();
            VerifyCounts(0, 0, 0, 0);
        }

        void RunTest() {
            myStoryTest = new StoryTest(table, (t,c) => { resultCounts = c;});
            myStoryTest.Execute(configuration);
        }

        [Test]
        public void TestOneExpectedOneActualCorrect()
        {
            const string name = "Joe";
            AddQueryValue(new RowFixturePerson(name));
            AddRow(new[] {name});
            RunTest();
            VerifyCounts(1, 0, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Right);
        }

        [Test]
        public void TestOneExpectedOneActualCorrectTwoColumns()
        {
            AddColumn(table, "address");
            const string name = "Joe";
            const string address = "First Street";
            AddQueryValue(new RowFixturePerson(name, address));
            AddRow(new[] {name, address});
            RunTest();
            VerifyCounts(2, 0, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Right);
            AssertTextInTag(table.At(0, 2, 1), TestStatus.Right);
        }

        [Test]
        public void TestTwoColumnAsKeyAllCorrect()
        {
            AddColumn(table, "address");
            AddColumn(table, "phone?");
            AddRow(new[] {"Joe", "First Street", "123-1234"});
            AddRow(new[] {"Joe", "Second Street", "234-2345"});
            AddQueryValue(new RowFixturePerson("Joe", "First Street", "123-1234"));
            AddQueryValue(new RowFixturePerson("Joe", "Second Street", "234-2345"));
            RunTest();
            VerifyCounts(6, 0, 0, 0);
        }

        [Test]
        public void TestTwoColumnAsKeyThirdColumnIncorrect()
        {
            AddColumn(table, "address");
            AddColumn(table, "phone?");
            AddRow(new[] {"Joe", "First Street", "123-1234"});
            AddRow(new[] {"Joe", "Second Street", "234-2345"});
            AddQueryValue(new RowFixturePerson("Joe", "First Street", "123-1234"));
            AddQueryValue(new RowFixturePerson("Joe", "Second Street", "234-2346"));
            RunTest();
            VerifyCounts(5, 1, 0, 0);
        }

        [Test]
        public void TestOneExpectedOneActualCorrectTwoColumnsSecondColumnWrong()
        {
            AddColumn(table, "address?");
            const string name = "Joe";
            AddQueryValue(new RowFixturePerson(name, "First Street"));
            AddRow(new[] {name, "Second Street"});
            RunTest();
            VerifyCounts(1, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Right);
            AssertTextInTag(table.At(0, 2, 1), TestStatus.Wrong);
        }

        [Test]
        public void TestOneExpectedOneActualIncorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddRow(new[] {"John"});
            RunTest();
            VerifyCounts(0, 2, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Wrong);
            AssertTextInBody(table.At(0, 2, 0), "missing");
            AssertTextInTag(table.At(0, 3, 0), TestStatus.Wrong);
            AssertTextInBody(table.At(0, 3, 0), "surplus");
        }

        [Test]
        public void TestDupsAllowed()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Joe"));
            AddRow(new[] {"Joe"});
            AddRow(new[] {"Joe"});
            RunTest();
            VerifyCounts(2, 0, 0, 0);
        }

        [Test]
        public void ThreeItemsWithCommonParts()
        {
            AddColumn(table, "address");
            AddQueryValue(new RowFixturePerson("A", "2"));
            AddQueryValue(new RowFixturePerson("B", "1"));
            AddQueryValue(new RowFixturePerson("A", "1"));
            AddRow(new[] {"A", "1"});
            AddRow(new[] {"A", "2"});
            AddRow(new[] {"B", "1"});
            RunTest();
            VerifyCounts(6, 0, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualAllCorrectOrderCorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new[] {"Joe"});
            AddRow(new[] {"Jane"});
            RunTest();
            VerifyCounts(2, 0, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualAllCorrectOrderIncorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new[] {"Jane"});
            AddRow(new[] {"Joe"});
            RunTest();
            VerifyCounts(2, 0, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualOneCorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new[] {"Joe"});
            AddRow(new[] {"Susan"});
            RunTest();
            VerifyCounts(1, 2, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualOneCorrectOrderIncorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new[] {"Susan"});
            AddRow(new[] {"Joe"});
            RunTest();
            VerifyCounts(1, 2, 0, 0);
        }

        [Test]
        public void TestOneMissing()
        {
            AddRow(new[] {"Joe"});
            RunTest();
            VerifyCounts(0, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Wrong);
            AssertTextInBody(table.At(0, 2, 0), "missing");
        }

        [Test]
        public void TestOneMissingTwoColumns()
        {
            AddColumn(table, "address");
            AddRow(new[] {"Joe", "First Street"});
            RunTest();
            VerifyCounts(0, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Wrong);
            AssertTextInBody(table.At(0, 2, 0), "missing");
        }

        [Test]
        public void TestOnePresentOneMissingTwoColumns()
        {
            AddColumn(table, "address");
            AddRow(new[] {"Lilian", "First Street"});
            AddRow(new[] {"Joe", "Second Street"});
            AddQueryValue(new RowFixturePerson("Lilian", "First Street"));
            RunTest();
            VerifyCounts(2, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Right);
            AssertTextInTag(table.At(0, 2, 1), TestStatus.Right);
            AssertTextNotInBody(table.At(0, 2, 0), "missing");
            AssertTextInTag(table.At(0, 3, 0), TestStatus.Wrong);
            AssertTextInBody(table.At(0, 3, 0), "missing");
        }

        [Test]
        public void TestOnePresentOneMissingTwoColumnsReverseOrder()
        {
            AddColumn(table, "address");
            AddRow(new[] {"Joe", "Second Street"});
            AddRow(new[] {"Lilian", "First Street"});
            AddQueryValue(new RowFixturePerson("Lilian", "First Street"));
            RunTest();
            VerifyCounts(2, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), TestStatus.Wrong);
            AssertTextInBody(table.At(0, 2, 0), "missing");
            AssertTextNotInBody(table.At(0, 2, 1), "missing");
            AssertTextInTag(table.At(0, 3, 0), TestStatus.Right);
            AssertTextInTag(table.At(0, 3, 1), TestStatus.Right);
            AssertTextNotInBody(table.At(0, 3, 0), "missing");
            AssertTextNotInBody(table.At(0, 3, 1), "missing");
        }

        [Test]
        public void TestCorrectFormatForMissing()
        {
            PeopleLoaderFixture.people.Clear();
            const string loaderFixtureHtml = "<table>" +
                                             "<tr><td colspan=\"3\">people loader fixture</td></tr>" +
                                             "<tr><td>id</td><td>first name</td><td>last name</td></tr>" +
                                             "<tr><td>1</td><td>null</td><td>Jones</td></tr>" +
                                             "<tr><td>2</td><td>Phil</td><td>blank</td></tr>" +
                                             "</table>";
            const string inspectorFixtureHtml = "<table>" +
                                                "<tr><td colspan=\"3\">people row fixture</td></tr>" +
                                                "<tr><td>id</td><td>first name</td><td>last name</td></tr>" +
                                                "<tr><td>7</td><td>nullest</td><td>Jonesey</td></tr>" +
                                                "<tr><td>2</td><td>Phil</td><td>blank</td></tr>" +
                                                "</table>";
            const string processedInspectorFixtureHtml = "<table>" +
                                                         "<tr><td colspan=\"3\">people row fixture</td></tr>" +
                                                         "<tr><td>id</td><td>first name</td><td>last name</td></tr>" +
                                                         "<tr><td class=\"fail\">7 <span class=\"fit_label\">missing</span></td><td>nullest</td><td>Jonesey</td></tr>" +
                                                         "<tr><td class=\"pass\">2</td><td class=\"pass\">Phil</td><td class=\"pass\">blank</td></tr>" +
                                                         "\n<tr>\n<td class=\"fail\"><span class=\"fit_grey\">1</span> <span class=\"fit_label\">surplus</span></td>\n<td><span class=\"fit_grey\">null</span></td>\n<td><span class=\"fit_grey\">Jones</span></td></tr>" +
                                                         "</table>";
            var tables = Parse.ParseFrom(loaderFixtureHtml + inspectorFixtureHtml);
            RunTest(tables);
            Assert.AreEqual(loaderFixtureHtml + processedInspectorFixtureHtml, tables.ToString());
        }

        [Test]
        public void TestArrayOfStrings()
        {
            ArrayOfStringsRowFixture.items.Clear();
            const string setUpTableHtml = "<table>" +
                                          "<tr><td colspan=\"3\">ArrayOfStringsFixture</td></tr>" +
                                          "<tr><td>field</td><td>save!</td></tr>" +
                                          "<tr><td>a,b,c</td><td></td></tr>" +
                                          "</table>";
            const string processedSetUpTableHtml = "<table>" +
                                                   "<tr><td colspan=\"3\">ArrayOfStringsFixture</td></tr>" +
                                                   "<tr><td>field</td><td>save!</td></tr>" +
                                                   "<tr><td>a,b,c</td><td><span class=\"fit_grey\"> null</span></td></tr>" +
                                                   "</table>";
            const string tableHtml = "<table>" +
                                     "<tr><td colspan=\"3\">ArrayOfStringsRowFixture</td></tr>" +
                                     "<tr><td>field</td></tr>" +
                                     "<tr><td>a,b,c</td></tr>" +
                                     "</table>";
            const string expected = "<table>" +
                                    "<tr><td colspan=\"3\">ArrayOfStringsRowFixture</td></tr>" +
                                    "<tr><td>field</td></tr>" +
                                    "<tr><td class=\"pass\">a,b,c</td></tr>" +
                                    "</table>";
            var tables = Parse.ParseFrom(setUpTableHtml + tableHtml);
            RunTest(tables);
            Assert.AreEqual(processedSetUpTableHtml + expected, tables.ToString());
        }

        [Test]
        public void TestEnum()
        {
            const string tableHtml = "<table><tr><td>ColorInspector</td></tr>" +
                                     "<tr><td>ToString()</td></tr>" +
                                     "<tr><td>Red</td></tr>" +
                                     "<tr><td>Blue</td></tr>" +
                                     "</table>";
            Array colorsArray = Enum.GetValues(typeof (Color));
            var colorsList = new ArrayList(colorsArray);
            DoTable(Parse.ParseFrom(tableHtml), colorsList.ToArray(), 2, 0, 0, 0);
        }

        void VerifyCounts(int right, int wrong, int exceptions, int ignores)
        {
            TestUtils.CheckCounts(resultCounts, right, wrong, exceptions, ignores);
        }

        static void AddQueryValue(object obj)
        {
            NewRowFixtureDerivative.QueryValues.Add(obj);
        }

        void AddRow(string[] strings)
        {
            var lastCell = new Parse("td", strings[strings.Length - 1], null, null);
            for (int i = strings.Length - 1; i > 0; i--)
            {
                lastCell = new Parse("td", strings[i - 1], null, lastCell);
            }
            table.Parts.Last.More = new Parse("tr", null, lastCell, null);
        }

        static void AssertTextInTag(Cell cell, string text)
        {
            Assert.AreEqual(text, cell.GetAttribute(CellAttribute.Status));
        }

        static void AssertTextInBody(Parse cell, string text)
        {
            Assert.IsTrue(cell.Body.IndexOf(text) > -1);
        }

        static void AssertTextNotInBody(Parse cell, string text)
        {
            Assert.IsFalse(cell.Body.IndexOf(text) > -1);
        }

        static void AddColumn(Parse table, string name)
        {
            table.Parts.More.Parts.Last.More = new Parse("td", name, null, null);
        }
    }

    public class BusinessObject
    {
        readonly string[] strs;

        public BusinessObject(string[] strs)
        {
            this.strs = strs;
        }

        public string[] GetStrings()
        {
            return strs;
        }

        public string GetFirstString()
        {
            return strs[0];
        }
    }

    public class BusinessObjectRowFixture : RowFixture
    {
        public static object[] objects;

        public override object[] Query()
        {
            return objects;
        }

        public override Type GetTargetClass()
        {
            return typeof (BusinessObject);
        }
    }

    public class NewRowFixtureDerivative : RowFixture
    {
        public static ArrayList QueryValues = new ArrayList();

        public override object[] Query()
        {
            return QueryValues.ToArray();
        }

        public override Type GetTargetClass()
        {
            return typeof (RowFixturePerson);
        }
    }

    public class RowFixturePerson
    {
        public RowFixturePerson(string name)
        {
            Name = name;
        }

        public RowFixturePerson(string name, string address)
        {
            Name = name;
            Address = address;
        }

        public RowFixturePerson(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;
        }

        public string Name { get; private set; }

        public string Address { get; private set; }

        public string Phone { get; private set; }
    }
}
