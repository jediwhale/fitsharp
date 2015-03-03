// Copyright © 2013 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Fixtures;
using fitSharp.Machine.Application;
using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class FlowKeywordsTest {
        private TestDoFixture fixture;
        private FlowKeywords keywords;

        [SetUp] public void SetUp() {
            fixture = new TestDoFixture { Processor = new Service.Service() };
            keywords = new FlowKeywords(fixture, fixture.Processor);
        }

        [Test]
        public void GherkinKeywords(
            [Values("Given", "When", "Then", "And", "But")] string keyword,
            [Values(true, false)] bool expectedResult)
        {
            var html = string.Format("<table><tr><td>{0}</td><td>Always{1}</td></tr></table>", keyword, expectedResult);
            var table = Parse.ParseFrom(html);
            bool result = true;
            switch (keyword)
            {
                case "Given":
                    result = keywords.Given(table.Parts.Parts);
                    break;
                case "When":
                    result = keywords.When(table.Parts.Parts);
                    break;
                case "Then":
                    result = keywords.Then(table.Parts.Parts);
                    break;
                case "And":
                    result = keywords.And(table.Parts.Parts);
                    break;
                case "But":
                    result = keywords.But(table.Parts.Parts);
                    break;
                default:
                    Assert.Fail("Unknown case: " + keyword);
                    break;
            }

            Assert.AreEqual(expectedResult, result, "Expected methods to return " + expectedResult);
        }

        [Test] public void NameKeywordAssignsASymbol() {
            var table = Parse.ParseFrom("<table><tr><td>name</td><td>symbol</td><td>stuff</td></tr></table>");
            keywords.Name(table.Parts.Parts);
            Assert.AreEqual("some stuff", fixture.NamedFixture("symbol"));
            Assert.AreEqual("some stuff", fixture.Processor.Get<Symbols>().GetValue("symbol"));
        }

        [Test] public void ShowAsKeywordComposesWithAttributes() {
            var table = Parse.ParseFrom("<table><tr><td>show as</td><td>raw</td><td>stuff</td></tr></table>");
            keywords.ShowAs(table.Parts.Parts);
            Assert.IsTrue(table.Parts.Parts.Last.HasAttribute(CellAttribute.Raw));
        }

        [Test] public void CheckFieldsForWrapsResultInList() {
            var table = Parse.ParseFrom("<table><tr><td>check fields</td><td>stuff</td></tr></table>");
            var result = keywords.CheckFieldsFor(table.Parts.Parts);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("some stuff", result[0]);
        }

        [Test] public void ChecksExpectedValue() {
            var table = Parse.ParseFrom("<table><tr><td>check</td><td>stuff</td><td>some stuff</td></tr></table>");
            keywords.Check(table.Parts.Parts);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Right, table.Parts.Parts.Last.GetAttribute(CellAttribute.Status));
        }

        [Test] public void WaitUntilRepeatsExpectedValueCheck() {
            var table = Parse.ParseFrom("<table><tr><td>wait until</td><td>next count</td><td>2</td></tr></table>");
            keywords.WaitUntil(table.Parts.Parts);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Right, table.Parts.Parts.Last.GetAttribute(CellAttribute.Status));
        }

        [Test] public void WaitUntilRepeatsUpToLimit() {
            fixture.Processor.Get<Symbols>().Save("WaitFor.Count", 10);
            fixture.Processor.Get<Symbols>().Save("WaitFor.Time", 1);
            var table = Parse.ParseFrom("<table><tr><td>wait until</td><td>next count</td><td>101</td></tr></table>");
            keywords.WaitUntil(table.Parts.Parts);
            var argumentPosition = 1;
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Wrong, table.Parts.Parts.Last.GetAttribute(CellAttribute.Status));
        }

        private class TestDoFixture: DoFixture {
            private const string stuff = "some stuff";
            public string Stuff = stuff;
            public bool AlwaysTrue = true;
            public bool AlwaysFalse = false;
            private int count;
            public int NextCount() {
                return count++;
            }
        }
    }
}
