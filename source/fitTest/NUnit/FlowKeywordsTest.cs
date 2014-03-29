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
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Wrong, table.Parts.Parts.Last.GetAttribute(CellAttribute.Status));
        }

        private class TestDoFixture: DoFixture {
            public string Stuff = "some stuff";
            private int count;
            public int NextCount() {
                return count++;
            }
        }
    }
}
