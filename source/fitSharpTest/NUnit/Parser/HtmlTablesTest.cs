// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Machine.Model;
using fitSharp.Parser;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture] public class HtmlTablesTest {

        [Test] public void ParseEmpty() {
            var result = Parse(string.Empty);
            Assert.IsTrue(result.Branches.Count == 0);
        }

        private static Tree<Cell> Parse(string input) {
            return new HtmlTables(text => new TreeList<Cell>(new CellBase(text))).Parse(input);
        }

        [Test] public void ParseNoTables() {
            var result = Parse("set the table");
            Assert.IsTrue(result.Branches.Count == 0);
            result = Parse("set the <table");
            Assert.IsTrue(result.Branches.Count == 0);
        }
    
        [Test, ExpectedException(typeof(ApplicationException))] public void ParseEmptyTable() {
            Parse("leader<table x=\"y\"></table>trailer");
        }
    
        [Test, ExpectedException(typeof(ApplicationException))] public void ParseTableWithBody() {
            Parse("leader<Table foo=2>body</table>trailer");
        }
    
        [Test] public void ParseTwoTables() {
            string result = Format(Parse("<table><tr><td>x</td></tr></table>leader<table><tr><td>x</td></tr></table>trailer"), " ");
            Assert.AreEqual(" <table> <tr> <td> x</td></tr></table> leader <table> <tr> <td> x</td></tr></table> trailer", result);
        }

        [Test, ExpectedException(typeof(ApplicationException))] public void ParseRow() {
            Parse(" <table>leader<tr></tr></table>");
        }
    
        [Test] public void ParseCell() {
            string result = Format(Parse("<table x=\"y\"><tr><td>content</td></tr></table>"), " ");
            Assert.AreEqual(" <table x=\"y\"> <tr> <td> content</td></tr></table>", result);
        }
    
        [Test] public void ParseRowWithCellAndBody() {
            string result = Format(Parse("<table><tr><td>content</td>somebody</tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> content</td> somebody</tr></table>", result);
        }
    
        [Test] public void ParseCellMixedCase() {
            string result = Format(Parse("leader<table><TR><Td>body</tD></TR></table>trailer"), " ");
            Assert.AreEqual(" leader <table> <TR> <Td> body</tD></TR></table> trailer", result);
        }
    
        [Test] public void ParseNestedTable() {
            string result = Format(Parse("<table><tr><td><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }
    
        [Test] public void ParseTwoNestedTables() {
            string result = Format(Parse("<table><tr><td><table><tr><td>content</td></tr></table><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <table> <tr> <td> content</td></tr></table> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }
    
        [Test] public void ParseNestedTableAndBody() {
            string result = Format(Parse("<table><tr><td><table><tr><td>content</td></tr></table>somebody</td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <table> <tr> <td> content</td></tr></table> somebody</td></tr></table>", result);
        }
    
        [Test] public void ParseNestedTableAndSplitBody() {
            string result = Format(Parse("<table><tr><td>some<table><tr><td>content</td></tr></table>body</td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> some <table> <tr> <td> content</td></tr></table> body</td></tr></table>", result);
        }
    
        [Test] public void ParseNestedList() {
            string result = Format(Parse("<table><tr><td><ul><li>content</li></ul></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <ul> <li> content</li></ul></td></tr></table>", result);
        }
    
        [Test] public void ParseNestedListAndTable() {
            string result = Format(Parse("<table><tr><td><ul><li>content</li></ul><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <ul> <li> content</li></ul> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }

        [Test] public void ParseContentWithoutTables() {
            // This is somewhat redundant with ParseNoTables, but I added 
            // it to make sure I understand the behavior of Format()
            var result = Parse("<p>Hello world!</p>");
            Assert.AreEqual(string.Empty, Format(result, " "));
        }

        [Test] public void ParseCommentedTablesIgnored() {
            var result = Parse("leader<!--<table><tr><td>ignored</td></tr></table>-->trailer");
            Assert.AreEqual(string.Empty, Format(result, " "));
        }

        [Test] public void ParseCommentedTableAsLeader() {
            var result = Parse("<!--<table><tr><td>ignored</td></tr></table>--><table><tr><td>foo</td></tr></table>");
            Assert.AreEqual(" <!--<table><tr><td>ignored</td></tr></table>--> <table> <tr> <td> foo</td></tr></table>", Format(result, " "));
        }

        [Test] public void ParseCommentWedgedBetweenTables() {
            var result = Parse("<table><tr><td>leader</td></tr></table><!--<table><tr><td>ignored</td></tr></table>--><table><tr><td>trailer</td></tr></table>");
            Assert.AreEqual(" <table> <tr> <td> leader</td></tr></table> <!--<table><tr><td>ignored</td></tr></table>--> <table> <tr> <td> trailer</td></tr></table>", Format(result, " "));
        }

        [Test] public void ParseCommentTarnishedTableElement() {
            var result = Parse("<!--table><tr><td>ignored</td></tr></table--><table><tr><td>trailer</td></tr></table>");
            Assert.AreEqual(" <!--table><tr><td>ignored</td></tr></table--> <table> <tr> <td> trailer</td></tr></table>", Format(result, " "));
        }

        [Test] public void ParseCommentUnclosed() {
            var result = Parse("<table><tr><td>leader</td></tr></table><!--<table><tr><td>ignored</td></tr></table><table><tr><td>trailer</td></tr></table>");
            Assert.AreEqual(" <table> <tr> <td> leader</td></tr></table> <!--<table><tr><td>ignored</td></tr></table><table><tr><td>trailer</td></tr></table>", Format(result, " "));
        }

        [Test] public void ParseCommentInsideTable() {
            var result = Parse("<table><tr><td>first</td></tr><!--<tr><td>first</td></tr>--></table>");
            Assert.AreEqual(" <table> <tr> <td> first</td></tr> <!--<tr><td>first</td></tr>--></table>", Format(result, " "));
        }

        [Test]
        public void ParseCommentSeveralCommentBlocks() {
            var result = Parse("<!--first--><p>other content</p><!--<table><tr><td>first</td></tr></table>-->");
            Assert.AreEqual(string.Empty, Format(result, " "));
        }

        static string Format(Tree<Cell> theParseTree, string theSeparator)
        {
            var result = new StringBuilder();
            foreach (var branch in theParseTree.Branches)
                result.AppendFormat("{0}{1}", theSeparator, FormatCell(branch, theSeparator));
            return result.ToString();
        }

        static string FormatCell(Tree<Cell> theParseTree, string theSeparator) {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(theParseTree.Value.GetAttribute(CellAttribute.Leader)))
                result.AppendFormat("{0}{1}", theParseTree.Value.GetAttribute(CellAttribute.Leader), theSeparator);
            result.Append(theParseTree.Value.GetAttribute(CellAttribute.StartTag));
            result.Append(Format(theParseTree, theSeparator));
            if (!string.IsNullOrEmpty(theParseTree.Value.GetAttribute(CellAttribute.Body)))
                result.AppendFormat("{0}{1}", theSeparator, theParseTree.Value.GetAttribute(CellAttribute.Body));
            result.Append(theParseTree.Value.GetAttribute(CellAttribute.EndTag));
            if (!string.IsNullOrEmpty(theParseTree.Value.GetAttribute(CellAttribute.Trailer)))
                result.AppendFormat("{0}{1}", theSeparator, theParseTree.Value.GetAttribute(CellAttribute.Trailer));
            return result.ToString();
        }
    }
}
