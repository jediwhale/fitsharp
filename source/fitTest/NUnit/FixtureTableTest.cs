// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class FixtureTableTest {

        [Test] public void EmptyTablesMatch() {
            FixtureTable table1 = new FixtureTable(null);
            FixtureTable table2 = new FixtureTable(null);
            Assert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void EmptyAndNonEmptyDifferent() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse("<table><tr><td>actual</td></tr></table>"));
            FixtureTable table2 = new FixtureTable(null);
            Assert.AreEqual("expected: null, was '<table>'", table1.Differences(table2));
        }
    
        [Test] public void TableCellsDifferent() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same</td><td>actual</td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same</td><td>expected</td></tr></table>"));

            Assert.AreEqual("in <table>, in <tr>, in <td> body, expected: 'expected', was 'actual'", table1.Differences(table2));
        }
    
        [Test] public void TableCellTagsDifferent() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td class=\"actual\">same</td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same</td></tr></table>"));

            Assert.AreEqual("in <table>, in <tr>, expected: '<td>', was '<td class=\"actual\">'", table1.Differences(table2));
        }
    
        [Test] public void EmptyTableCellsMatch() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same</td><td></td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same</td><td></td></tr></table>"));

            Assert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceStartsWithExpectedMatches() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace blah blah</span></td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace</span></td></tr></table>>"));

            Assert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceWithoutExceptionNameMatches() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">x.y.z: stack: trace</span></td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack: trace</span></td></tr></table>>"));

            Assert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceEmptyExpectedMatches() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace blah blah</span></td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\"></span></td></tr></table>>"));

            Assert.AreEqual(string.Empty, table1.Differences(table2));
        }

        [Test] public void AlternateStackTraceStartsWithExpectedMatches() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace blah blah</span></td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_label\">stack trace</span></td></tr></table>>"));

            Assert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceNoneExpectedDifferent() {
            FixtureTable table1 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace blah blah</span></td></tr></table>"));

            FixtureTable table2 = new FixtureTable(HtmlParser.Instance.Parse(
                                                       "<table><tr><td>same</td></tr></table>>"));

            Assert.AreEqual("in <table>, in <tr>, in <td> body, expected: null, was 'stack trace blah blah'", table1.Differences(table2));
        }

    }
}