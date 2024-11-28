// Copyright � 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture] public class FixtureTableTest {

        [Test] public void EmptyTablesMatch() {
            var table1 = new FixtureTable(null);
            var table2 = new FixtureTable(null);
            ClassicAssert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void EmptyAndNonEmptyDifferent() {
            var table1 = new FixtureTable(Parse.ParseFrom("<table><tr><td>actual</td></tr></table>"));
            var table2 = new FixtureTable(null);
            ClassicAssert.AreEqual("expected: null, was '<table>'", table1.Differences(table2));
        }
    
        [Test] public void TableCellsDifferent() {
            var table1 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same</td><td>actual</td></tr></table>"));

            var table2 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same</td><td>expected</td></tr></table>"));

            ClassicAssert.AreEqual("in <table>, in <tr>, in <td> body, expected: 'expected', was 'actual'", table1.Differences(table2));
        }
    
        [Test] public void TableCellTagsDifferent() {
            var table1 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td class=\"actual\">same</td></tr></table>"));

            var table2 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same</td></tr></table>"));

            ClassicAssert.AreEqual("in <table>, in <tr>, expected: '<td>', was '<td class=\"actual\">'", table1.Differences(table2));
        }
    
        [Test] public void EmptyTableCellsMatch() {
            var table1 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same</td><td></td></tr></table>"));

            var table2 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same</td><td></td></tr></table>"));

            ClassicAssert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceStartsWithExpectedMatches() {
            var table1 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace blah blah</span></td></tr></table>"));

            var table2 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace</span></td></tr></table>>"));

            ClassicAssert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceWithoutExceptionNameMatches() {
            var table1 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">x.y.z: stack: trace</span></td></tr></table>"));

            var table2 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack: trace</span></td></tr></table>>"));

            ClassicAssert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceEmptyExpectedMatches() {
            var table1 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace blah blah</span></td></tr></table>"));

            var table2 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\"></span></td></tr></table>>"));

            ClassicAssert.AreEqual(string.Empty, table1.Differences(table2));
        }
    
        [Test] public void StackTraceNoneExpectedDifferent() {
            var table1 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same<span class=\"fit_stacktrace\">stack trace blah blah</span></td></tr></table>"));

            var table2 = new FixtureTable(Parse.ParseFrom(
                                                       "<table><tr><td>same</td></tr></table>>"));

            ClassicAssert.AreEqual("in <table>, in <tr>, in <td> body, expected: null, was 'stack trace blah blah'", table1.Differences(table2));
        }

    }
}