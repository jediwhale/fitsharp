// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Text;
using fit.Test.Acceptance;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class RowFixtureTest
    {
        public void TestExpectBlankOrNullAllCorrect()
        {
            TestUtils.InitAssembliesAndNamespaces();
            DoTable(
                BuildTable(new string[] {"null", "blank", "joe"}),
                BuildObjectArray(new string[] {null, "", "joe"}),
                3, 0, 0, 0
                );
            DoTable(
                BuildTable(new string[] {"Null", "Blank"}),
                BuildObjectArray(new string[] {null, ""}),
                2, 0, 0, 0
                );
            DoTable(
                BuildTable(new string[] {"NULL", "BLANK"}),
                BuildObjectArray(new string[] {null, ""}),
                2, 0, 0, 0
                );
        }

        public void TestExpectBlankOrNullSomeWrong()
        {
            TestUtils.InitAssembliesAndNamespaces();
            Parse table = BuildTable(new string[] {"blank", "null"});
            DoTable(
                table,
                BuildObjectArray(new string[] {"", "this is not null"}),
                1, 2, 0, 0
                );
        }

        private Parse BuildTable(string[] values)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>BusinessObjectRowFixture</td></tr>");
            builder.Append("<tr><td>GetFirstString</td></tr>");
            foreach (string value in values)
            {
                builder.Append("<tr><td>" + value + "</td></tr>");
            }
            builder.Append("</table>");
            return new Parse(builder.ToString());
        }

        private object[] BuildObjectArray(string[] values)
        {
            object[] objects = new object[values.Length];
            int count = 0;
            foreach (string value in values)
            {
                objects[count++] = new BusinessObject(new string[] {value});
            }
            return objects;
        }

        public void DoTable(Parse tables, object[] businessObjects, int right, int wrong, int ignores, int exceptions)
        {
            BusinessObjectRowFixture.objects = businessObjects;
            StoryTest test = new StoryTest(tables);
            test.Execute();

            Assert.AreEqual(right, test.Counts.Right);
            Assert.AreEqual(wrong, test.Counts.Wrong);
            Assert.AreEqual(ignores, test.Counts.Ignores);
            Assert.AreEqual(exceptions, test.Counts.Exceptions);
        }

        [Test]
        public void TestSurplus()
        {
            TestUtils.InitAssembliesAndNamespaces();
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>BusinessObjectRowFixture</td></tr>");
            builder.Append("<tr><td>GetFirstString</td></tr>");
            builder.Append("<tr><td>number1</td></tr>");
            builder.Append("</table>");
            Parse parse = new Parse(builder.ToString());

            BusinessObjectRowFixture.objects = new object[]
                                               {
                                                   new BusinessObject(new string[] {"number1"}),
                                                   new BusinessObject(new string[] {"number2"}),
                                                   new BusinessObject(new string[] {"number3"})
                                               };

            StoryTest test = new StoryTest(parse);
            test.Execute();
            Assert.IsTrue(parse.ToString().IndexOf("number1") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number2") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number3") > 0);
            Assert.AreEqual(1, test.Counts.Right);
            Assert.AreEqual(2, test.Counts.Wrong);
        }

        [Test]
        public void TestMissing()
        {
            TestUtils.InitAssembliesAndNamespaces();
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>BusinessObjectRowFixture</td></tr>");
            builder.Append("<tr><td>GetFirstString</td></tr>");
            builder.Append("<tr><td>number1</td></tr>");
            builder.Append("<tr><td>number2</td></tr>");
            builder.Append("<tr><td>number3</td></tr>");
            builder.Append("</table>");
            Parse parse = new Parse(builder.ToString());

            BusinessObjectRowFixture.objects = new object[]
                                               {
                                                   new BusinessObject(new string[] {"number1"}),
                                               };

            StoryTest test = new StoryTest(parse);
            test.Execute();
            Assert.IsTrue(parse.ToString().IndexOf("number1") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number2") > 0);
            Assert.IsTrue(parse.ToString().IndexOf("number3") > 0);
            Assert.AreEqual(2, test.Counts.Wrong);
        }

        [Test]
        public void TestStartsWithHandlerInSecondColumn()
        {
            TestUtils.InitAssembliesAndNamespaces();
            //???ObjectFactory.AddNamespace("fitnesse.Handlers");
           
            Context.Configuration.GetItem<Service.Service>().AddOperator(typeof(CompareStartsWith).FullName);
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td>people row fixture</td></tr>");
            builder.Append("<tr><td>first name</td><td>last name</td></tr>");
            builder.Append("<tr><td>Nigel</td><td>Tuf..</td></tr>");
            builder.Append("</table>");
            PeopleLoaderFixture.people.Clear();
            PeopleLoaderFixture.people.Add(new Person("Nigel", "Tufnel"));
            Parse tables = new Parse(builder.ToString());
            StoryTest test = new StoryTest(tables);
            test.Execute();
            Assert.IsTrue(tables.ToString().IndexOf("Tuf..") > -1);
            Assert.IsFalse(tables.ToString().IndexOf("Tufnel") > -1);
            Assert.AreEqual(2, test.Counts.Right);
            Assert.AreEqual(0, test.Counts.Wrong);
            Assert.AreEqual(0, test.Counts.Ignores);
            Assert.AreEqual(0, test.Counts.Exceptions);
        }

        private string rowFixtureName = typeof (NewRowFixtureDerivative).Name;
        private Parse table;
        private StoryTest myStoryTest;

        [SetUp]
        public void SetUp()
        {
            TestUtils.InitAssembliesAndNamespaces();
            table = new Parse("<table><tr><td>" + rowFixtureName + "</td></tr><tr><td>name</td></tr></table>");
            NewRowFixtureDerivative.QueryValues.Clear();
        }

        [Test]
        public void TestZeroExpectedZeroActual()
        {
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(0, 0, 0, 0);
        }

        [Test]
        public void TestOneExpectedOneActualCorrect()
        {
            string name = "Joe";
            AddQueryValue(new RowFixturePerson(name));
            AddRow(new string[] {name});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(1, 0, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.PassStatus);
        }

        [Test]
        public void TestOneExpectedOneActualCorrectTwoColumns()
        {
            AddColumn(table, "address");
            string name = "Joe";
            string address = "First Street";
            AddQueryValue(new RowFixturePerson(name, address));
            AddRow(new string[] {name, address});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(2, 0, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.PassStatus);
            AssertTextInTag(table.At(0, 2, 1), CellAttributes.PassStatus);
        }

        [Test]
        public void TestTwoColumnAsKeyAllCorrect()
        {
            AddColumn(table, "address");
            AddColumn(table, "phone?");
            AddRow(new string[] {"Joe", "First Street", "123-1234"});
            AddRow(new string[] {"Joe", "Second Street", "234-2345"});
            AddQueryValue(new RowFixturePerson("Joe", "First Street", "123-1234"));
            AddQueryValue(new RowFixturePerson("Joe", "Second Street", "234-2345"));
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(6, 0, 0, 0);
        }

        [Test]
        public void TestTwoColumnAsKeyThirdColumnIncorrect()
        {
            AddColumn(table, "address");
            AddColumn(table, "phone?");
            AddRow(new string[] {"Joe", "First Street", "123-1234"});
            AddRow(new string[] {"Joe", "Second Street", "234-2345"});
            AddQueryValue(new RowFixturePerson("Joe", "First Street", "123-1234"));
            AddQueryValue(new RowFixturePerson("Joe", "Second Street", "234-2346"));
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(5, 1, 0, 0);
        }

        [Test]
        public void TestOneExpectedOneActualCorrectTwoColumnsSecondColumnWrong()
        {
            AddColumn(table, "address?");
            string name = "Joe";
            AddQueryValue(new RowFixturePerson(name, "First Street"));
            AddRow(new string[] {name, "Second Street"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(1, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.PassStatus);
            AssertTextInTag(table.At(0, 2, 1), CellAttributes.FailStatus);
        }

        [Test]
        public void TestOneExpectedOneActualIncorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddRow(new string[] {"John"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(0, 2, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.FailStatus);
            AssertTextInBody(table.At(0, 2, 0), "missing");
            AssertTextInTag(table.At(0, 3, 0), CellAttributes.FailStatus);
            AssertTextInBody(table.At(0, 3, 0), "surplus");
        }

        [Test]
        public void TestDupsAllowed()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Joe"));
            AddRow(new string[] {"Joe"});
            AddRow(new string[] {"Joe"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(2, 0, 0, 0);
        }

        [Test]
        public void ThreeItemsWithCommonParts()
        {
            AddColumn(table, "address");
            AddQueryValue(new RowFixturePerson("A", "2"));
            AddQueryValue(new RowFixturePerson("B", "1"));
            AddQueryValue(new RowFixturePerson("A", "1"));
            AddRow(new string[] {"A", "1"});
            AddRow(new string[] {"A", "2"});
            AddRow(new string[] {"B", "1"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(6, 0, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualAllCorrectOrderCorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new string[] {"Joe"});
            AddRow(new string[] {"Jane"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(2, 0, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualAllCorrectOrderIncorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new string[] {"Jane"});
            AddRow(new string[] {"Joe"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(2, 0, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualOneCorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new string[] {"Joe"});
            AddRow(new string[] {"Susan"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(1, 2, 0, 0);
        }

        [Test]
        public void TestTwoExpectedTwoActualOneCorrectOrderIncorrect()
        {
            AddQueryValue(new RowFixturePerson("Joe"));
            AddQueryValue(new RowFixturePerson("Jane"));
            AddRow(new string[] {"Susan"});
            AddRow(new string[] {"Joe"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(1, 2, 0, 0);
        }

        [Test]
        public void TestOneMissing()
        {
            AddRow(new string[] {"Joe"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(0, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.FailStatus);
            AssertTextInBody(table.At(0, 2, 0), "missing");
        }

        [Test]
        public void TestOneMissingTwoColumns()
        {
            AddColumn(table, "address");
            AddRow(new string[] {"Joe", "First Street"});
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(0, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.FailStatus);
            AssertTextInBody(table.At(0, 2, 0), "missing");
        }

        [Test]
        public void TestOnePresentOneMissingTwoColumns()
        {
            AddColumn(table, "address");
            AddRow(new string[] {"Lilian", "First Street"});
            AddRow(new string[] {"Joe", "Second Street"});
            AddQueryValue(new RowFixturePerson("Lilian", "First Street"));
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(2, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.PassStatus);
            AssertTextInTag(table.At(0, 2, 1), CellAttributes.PassStatus);
            AssertTextNotInBody(table.At(0, 2, 0), "missing");
            AssertTextInTag(table.At(0, 3, 0), CellAttributes.FailStatus);
            AssertTextInBody(table.At(0, 3, 0), "missing");
        }

        [Test]
        public void TestOnePresentOneMissingTwoColumnsReverseOrder()
        {
            AddColumn(table, "address");
            AddRow(new string[] {"Joe", "Second Street"});
            AddRow(new string[] {"Lilian", "First Street"});
            AddQueryValue(new RowFixturePerson("Lilian", "First Street"));
            myStoryTest = new StoryTest(table);
            myStoryTest.Execute();
            VerifyCounts(2, 1, 0, 0);
            AssertTextInTag(table.At(0, 2, 0), CellAttributes.FailStatus);
            AssertTextInBody(table.At(0, 2, 0), "missing");
            AssertTextNotInBody(table.At(0, 2, 1), "missing");
            AssertTextInTag(table.At(0, 3, 0), CellAttributes.PassStatus);
            AssertTextInTag(table.At(0, 3, 1), CellAttributes.PassStatus);
            AssertTextNotInBody(table.At(0, 3, 0), "missing");
            AssertTextNotInBody(table.At(0, 3, 1), "missing");
        }

        [Test]
        public void TestCorrectFormatForMissing()
        {
            PeopleLoaderFixture.people.Clear();
            string loaderFixtureHtml = "<table>" +
                                       "<tr><td colspan=\"3\">people loader fixture</td></tr>" +
                                       "<tr><td>id</td><td>first name</td><td>last name</td></tr>" +
                                       "<tr><td>1</td><td>null</td><td>Jones</td></tr>" +
                                       "<tr><td>2</td><td>Phil</td><td>blank</td></tr>" +
                                       "</table>";
            string inspectorFixtureHtml = "<table>" +
                                          "<tr><td colspan=\"3\">people row fixture</td></tr>" +
                                          "<tr><td>id</td><td>first name</td><td>last name</td></tr>" +
                                          "<tr><td>7</td><td>nullest</td><td>Jonesey</td></tr>" +
                                          "<tr><td>2</td><td>Phil</td><td>blank</td></tr>" +
                                          "</table>";
            string processedInspectorFixtureHtml = "<table>" +
                                                   "<tr><td colspan=\"3\">people row fixture</td></tr>" +
                                                   "<tr><td>id</td><td>first name</td><td>last name</td></tr>" +
                                                   "<tr><td class=\"fail\">7 <span class=\"fit_label\">missing</span></td><td>nullest</td><td>Jonesey</td></tr>" +
                                                   "<tr><td class=\"pass\">2</td><td class=\"pass\">Phil</td><td class=\"pass\">blank</td></tr>" +
                                                   "\n<tr>\n<td class=\"fail\"><span class=\"fit_grey\">1</span> <span class=\"fit_label\">surplus</span></td>\n<td><span class=\"fit_grey\">null</span></td>\n<td><span class=\"fit_grey\">Jones</span></td></tr>" +
                                                   "</table>";
            Parse tables = new Parse(loaderFixtureHtml + inspectorFixtureHtml);
            myStoryTest = new StoryTest(tables);
            myStoryTest.Execute();
            Assert.AreEqual(loaderFixtureHtml + processedInspectorFixtureHtml, tables.ToString());
        }

        [Test]
        public void TestArrayOfStrings()
        {
            ArrayOfStringsRowFixture.items.Clear();
            string setUpTableHtml =
                //"<table>" +
                //"<tr><td>configuration setup</td></tr>" +
                //"<tr><td>service</td></tr>" +
                //"<tr><td>add operator</td><td>fit.operators.executeempty</td></tr>" +
                //"</table>" +
                "<table>" +
                "<tr><td colspan=\"3\">ArrayOfStringsFixture</td></tr>" +
                "<tr><td>field</td><td>save!</td></tr>" +
                "<tr><td>a,b,c</td><td></td></tr>" +
                "</table>";
            string processedSetUpTableHtml =
                //"<table>" +
                //"<tr><td>configuration setup</td></tr>" +
                //"<tr><td>service</td></tr>" +
                //"<tr><td>add operator</td><td>fit.operators.executeempty</td></tr>" +
                //"</table>" +
                "<table>" +
                "<tr><td colspan=\"3\">ArrayOfStringsFixture</td></tr>" +
                "<tr><td>field</td><td>save!</td></tr>" +
                "<tr><td>a,b,c</td><td><span class=\"fit_grey\"> null</span></td></tr>" +
                "</table>";
            string tableHtml = "<table>" +
                               "<tr><td colspan=\"3\">ArrayOfStringsRowFixture</td></tr>" +
                               "<tr><td>field</td></tr>" +
                               "<tr><td>a,b,c</td></tr>" +
                               "</table>";
            string expected = "<table>" +
                              "<tr><td colspan=\"3\">ArrayOfStringsRowFixture</td></tr>" +
                              "<tr><td>field</td></tr>" +
                              "<tr><td class=\"pass\">a,b,c</td></tr>" +
                              "</table>";
            Parse tables = new Parse(setUpTableHtml + tableHtml);
            myStoryTest = new StoryTest(tables);
            myStoryTest.Execute();
            var x = tables.ToString();
            Assert.AreEqual(processedSetUpTableHtml + expected, tables.ToString());
        }

        [Test]
        public void TestEnum()
        {
            string tableHtml = "<table><tr><td>ColorInspector</td></tr>" +
                               "<tr><td>ToString()</td></tr>" +
                               "<tr><td>Red</td></tr>" +
                               "<tr><td>Blue</td></tr>" +
                               "</table>";
            Array colorsArray = Enum.GetValues(typeof (Color));
            ArrayList colorsList = new ArrayList(colorsArray);
            DoTable(new Parse(tableHtml), colorsList.ToArray(), 2, 0, 0, 0);
        }

        private void VerifyCounts(int right, int wrong, int exceptions, int ignores)
        {
            Assert.AreEqual(right, myStoryTest.Counts.Right);
            Assert.AreEqual(wrong, myStoryTest.Counts.Wrong);
            Assert.AreEqual(exceptions, myStoryTest.Counts.Exceptions);
            Assert.AreEqual(ignores, myStoryTest.Counts.Ignores);
        }

        private void AddQueryValue(object obj)
        {
            NewRowFixtureDerivative.QueryValues.Add(obj);
        }

        private void AddRow(string[] strings)
        {
            Parse lastCell = new Parse("td", strings[strings.Length - 1], null, null);
            for (int i = strings.Length - 1; i > 0; i--)
            {
                lastCell = new Parse("td", strings[i - 1], null, lastCell);
            }
            table.Parts.Last.More = new Parse("tr", null, lastCell, null);
        }

        private void AssertTextInTag(Parse cell, string text)
        {
            Assert.AreEqual(text, cell.GetAttribute(CellAttributes.StatusKey));
        }

        private void AssertTextInBody(Parse cell, string text)
        {
            Assert.IsTrue(cell.Body.IndexOf(text) > -1);
        }

        private void AssertTextNotInBody(Parse cell, string text)
        {
            Assert.IsFalse(cell.Body.IndexOf(text) > -1);
        }

        private void AddColumn(Parse table, string name)
        {
            table.Parts.More.Parts.Last.More = new Parse("td", name, null, null);
        }
    }

    public class BusinessObject
    {
        private string[] strs;

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
            this.name = name;
        }

        public RowFixturePerson(string name, string address)
        {
            this.name = name;
            this.address = address;
        }

        public RowFixturePerson(string name, string address, string phone)
        {
            this.name = name;
            this.address = address;
            this.phone = phone;
        }

        public string Name
        {
            get { return name; }
        }

        public string Address
        {
            get { return address; }
        }

        public string Phone
        {
            get { return phone; }
        }

        private string name;
        private string address;
        private string phone;
    }

}