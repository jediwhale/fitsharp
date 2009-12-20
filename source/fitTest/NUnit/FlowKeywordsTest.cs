// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Fixtures;
using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class FlowKeywordsTest {
        [Test] public void NameKeywordAssignsASymbol() {
            var fixture = new TestDoFixture { Processor = new CellProcessorBase() };
            var keywords = new FlowKeywords(fixture);
            Parse table = new HtmlParser().Parse("<table><tr><td>name</td><td>symbol</td><td>stuff</td></tr></table>");
            keywords.Name(table.Parts.Parts);
            Assert.AreEqual("some stuff", fixture.NamedFixture("symbol"));
            Assert.AreEqual("some stuff", fixture.Processor.Load(new Symbol("symbol")).Instance);
        }

        private class TestDoFixture: DoFixture {
            public string Stuff = "some stuff";
        }
    }
}
