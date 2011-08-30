// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Fixtures;
using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class FlowKeywordsTest {
        private TestDoFixture fixture;
        private FlowKeywords keywords;

        [SetUp] public void SetUp() {
            fixture = new TestDoFixture { Processor = new Service.Service() };
            keywords = new FlowKeywords(fixture);
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
            Assert.AreEqual("<table><tr><td>show as</td><td>raw</td><td>stuff</td>\n<td>some stuff</td></tr></table>", table.ToString());
        }

        private class TestDoFixture: DoFixture {
            public string Stuff = "some stuff";
        }
    }
}
