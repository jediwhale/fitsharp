// Copyright � 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Machine.Model;
using fitSharp.Parser;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture] public class HtmlTablesTest {

        [Test] public void ParseEmpty() {
            var result = Parse(string.Empty);
            ClassicAssert.IsTrue(result.Branches.Count == 0);
        }

        [Test] public void ParseNoTables() {
            var result = Parse("set the table");
            ClassicAssert.IsTrue(result.Branches.Count == 0);
            result = Parse("set the <table");
            ClassicAssert.IsTrue(result.Branches.Count == 0);
        }
    
        [Test] public void ParseEmptyTable() {
            ClassicAssert.Throws<ApplicationException>(() => Parse("leader<table x=\"y\"></table>trailer"));
        }
    
        [Test] public void ParseTableWithBody() {
            ClassicAssert.Throws<ApplicationException>(() =>Parse("leader<Table foo=2>body</table>trailer"));
        }
    
        [Test] public void ParseTwoTables() {
            AssertParse("<table><tr><td>x</td></tr></table>leader<table><tr><td>x</td></tr></table>trailer",
                " <table> <tr> <td> x</td></tr></table> leader <table> <tr> <td> x</td></tr></table> trailer");
        }

        [Test] public void ParseRow() {
            ClassicAssert.Throws<ApplicationException>(() =>Parse(" <table>leader<tr></tr></table>"));
        }
    
        [Test] public void ParseCell() {
            AssertParse("<table x=\"y\"><tr><td>content</td></tr></table>",
                " <table x=\"y\"> <tr> <td> content</td></tr></table>");
        }
    
        [Test] public void ParseRowWithCellAndBody() {
            AssertParse("<table><tr><td>content</td>somebody</tr></table>",
                " <table> <tr> <td> content</td> somebody</tr></table>");
        }
    
        [Test] public void ParseCellMixedCase() {
            AssertParse("leader<table><TR><Td>body</tD></TR></table>trailer",
                " leader <table> <TR> <Td> body</tD></TR></table> trailer");
        }
    
        [Test] public void ParseNestedTable() {
            AssertParse("<table><tr><td><table><tr><td>content</td></tr></table></td></tr></table>",
                " <table> <tr> <td> <table> <tr> <td> content</td></tr></table></td></tr></table>");
        }
    
        [Test] public void ParseTwoNestedTables() {
            AssertParse("<table><tr><td><table><tr><td>content</td></tr></table><table><tr><td>content</td></tr></table></td></tr></table>",
                " <table> <tr> <td> <table> <tr> <td> content</td></tr></table> <table> <tr> <td> content</td></tr></table></td></tr></table>");
        }
    
        [Test] public void ParseNestedTableAndBody() {
            AssertParse("<table><tr><td><table><tr><td>content</td></tr></table>somebody</td></tr></table>",
                " <table> <tr> <td> <table> <tr> <td> content</td></tr></table> somebody</td></tr></table>");
        }
    
        [Test] public void ParseNestedTableAndSplitBody() {
            AssertParse("<table><tr><td>some<table><tr><td>content</td></tr></table>body</td></tr></table>",
                " <table> <tr> <td> some <table> <tr> <td> content</td></tr></table> body</td></tr></table>");
        }
    
        [Test] public void ParseNestedList() {
            AssertParse("<table><tr><td><ul><li>content</li></ul></td></tr></table>",
                " <table> <tr> <td> <ul> <li> content</li></ul></td></tr></table>");
        }
    
        [Test] public void ParseNestedListAndTable() {
            AssertParse("<table><tr><td><ul><li>content</li></ul><table><tr><td>content</td></tr></table></td></tr></table>",
                " <table> <tr> <td> <ul> <li> content</li></ul> <table> <tr> <td> content</td></tr></table></td></tr></table>");
        }

        [Test] public void ParseContentWithoutTables() {
            // This is somewhat redundant with ParseNoTables, but I added 
            // it to make sure I understand the behavior of Format()
            AssertParse("<p>Hello world!</p>",
                string.Empty);
        }

        [Test] public void ParseCommentedTablesIgnored() {
            AssertParse("leader<!--<table><tr><td>ignored</td></tr></table>-->trailer",
                string.Empty);
        }

        [Test] public void ParseCommentedTableAsLeader() {
            AssertParse("<!--<table><tr><td>ignored</td></tr></table>--><table><tr><td>foo</td></tr></table>",
                " <!--<table><tr><td>ignored</td></tr></table>--> <table> <tr> <td> foo</td></tr></table>");
        }

        [Test] public void ParseCommentWedgedBetweenTables() {
            AssertParse("<table><tr><td>leader</td></tr></table><!--<table><tr><td>ignored</td></tr></table>--><table><tr><td>trailer</td></tr></table>",
                " <table> <tr> <td> leader</td></tr></table> <!--<table><tr><td>ignored</td></tr></table>--> <table> <tr> <td> trailer</td></tr></table>");
        }

        [Test] public void ParseCommentTarnishedTableElement() {
            AssertParse("<!--table><tr><td>ignored</td></tr></table--><table><tr><td>trailer</td></tr></table>",
                " <!--table><tr><td>ignored</td></tr></table--> <table> <tr> <td> trailer</td></tr></table>");
        }

        [Test] public void ParseCommentUnclosed() {
            AssertParse("<table><tr><td>leader</td></tr></table><!--<table><tr><td>ignored</td></tr></table><table><tr><td>trailer</td></tr></table>",
                " <table> <tr> <td> leader</td></tr></table> <!--<table><tr><td>ignored</td></tr></table><table><tr><td>trailer</td></tr></table>");
        }

        [Test] public void ParseCommentInsideTable() {
            AssertParse("<table><tr><td>first</td></tr><!--<tr><td>first</td></tr>--></table>",
                " <table> <tr> <td> first</td></tr> <!--<tr><td>first</td></tr>--></table>");
        }

        [Test]
        public void ParseCommentSeveralCommentBlocks() {
            AssertParse("<!--first--><p>other content</p><!--<table><tr><td>first</td></tr></table>-->",
                string.Empty);
        }

        static void AssertParse(string input, string expected) {
            ClassicAssert.AreEqual("<div>" + expected + "</div>", Parse(input).Format());
        }

        static Tree<Cell> Parse(string input) {
            return new HtmlTables(text => new TreeList<Cell>(new CellBase(text))).Parse(input);
        }
    }
}
