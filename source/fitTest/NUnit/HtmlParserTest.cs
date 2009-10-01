// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Text;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class HtmlParserTest {

        [Test] public void ParseEmpty() {
            Parse result = HtmlParser.Instance.Parse(string.Empty);
            Assert.IsTrue(result == null);
        }
    
        [Test] public void ParseNoTables() {
            Parse result = HtmlParser.Instance.Parse("set the table");
            Assert.IsTrue(result == null);
            result = (new HtmlParser()).Parse("set the <table");
            Assert.IsTrue(result == null);
        }
    
        [Test, ExpectedException(typeof(ApplicationException))] public void ParseEmptyTable() {
            HtmlParser.Instance.Parse("leader<table x=\"y\"></table>trailer");
        }
    
        [Test, ExpectedException(typeof(ApplicationException))] public void ParseTableWithBody() {
            HtmlParser.Instance.Parse("leader<Table foo=2>body</table>trailer");
        }
    
        [Test] public void ParseTwoTables() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td>x</td></tr></table>leader<table><tr><td>x</td></tr></table>trailer"), " ");
            Assert.AreEqual("<table> <tr> <td> x</td></tr></table> leader <table> <tr> <td> x</td></tr></table> trailer", result);
        }

        [Test, ExpectedException(typeof(ApplicationException))] public void ParseRow() {
            HtmlParser.Instance.Parse("<table>leader<tr></tr></table>");
        }
    
        [Test] public void ParseCell() {
            string result = Format(HtmlParser.Instance.Parse("<table x=\"y\"><tr><td>content</td></tr></table>"), " ");
            Assert.AreEqual("<table x=\"y\"> <tr> <td> content</td></tr></table>", result);
        }
    
        [Test] public void ParseRowWithCellAndBody() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td>content</td>somebody</tr></table>"), " ");
            Assert.AreEqual("<table> <tr> <td> content</td> somebody</tr></table>", result);
        }
    
        [Test] public void ParseCellMixedCase() {
            string result = Format(HtmlParser.Instance.Parse("leader<table><TR><Td>body</tD></TR></table>trailer"), " ");
            Assert.AreEqual("leader <table> <TR> <Td> body</tD></TR></table> trailer", result);
        }
    
        [Test] public void ParseNestedTable() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual("<table> <tr> <td> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }
    
        [Test] public void ParseTwoNestedTables() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td><table><tr><td>content</td></tr></table><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual("<table> <tr> <td> <table> <tr> <td> content</td></tr></table> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }
    
        [Test] public void ParseNestedTableAndBody() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td><table><tr><td>content</td></tr></table>somebody</td></tr></table>"), " ");
            Assert.AreEqual("<table> <tr> <td> <table> <tr> <td> content</td></tr></table> somebody</td></tr></table>", result);
        }
    
        [Test] public void ParseNestedTableAndSplitBody() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td>some<table><tr><td>content</td></tr></table>body</td></tr></table>"), " ");
            Assert.AreEqual("<table> <tr> <td> some <table> <tr> <td> content</td></tr></table> body</td></tr></table>", result);
        }
    
        [Test] public void ParseNestedList() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td><ul><li>content</li></ul></td></tr></table>"), " ");
            Assert.AreEqual("<table> <tr> <td> <ul> <li> content</li></ul></td></tr></table>", result);
        }
    
        [Test] public void ParseNestedListAndTable() {
            string result = Format(HtmlParser.Instance.Parse("<table><tr><td><ul><li>content</li></ul><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual("<table> <tr> <td> <ul> <li> content</li></ul> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }

        private static string Format(Parse theParseTree, string theSeparator) {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(theParseTree.Leader)) result.AppendFormat("{0}{1}", theParseTree.Leader, theSeparator);
            result.Append(theParseTree.Tag);
            if (theParseTree.Parts != null) result.AppendFormat("{0}{1}", theSeparator, Format(theParseTree.Parts, theSeparator));
            if (!string.IsNullOrEmpty(theParseTree.Body)) result.AppendFormat("{0}{1}", theSeparator, theParseTree.Body);
            result.Append(theParseTree.End);
            if (!string.IsNullOrEmpty(theParseTree.Trailer)) result.AppendFormat("{0}{1}", theSeparator, theParseTree.Trailer);
            if (theParseTree.More != null) result.AppendFormat("{0}{1}", theSeparator, Format(theParseTree.More, theSeparator));
            return result.ToString();
        }
    }
}