// Copyright © 2010 Syterra Software Inc. All rights reserved.
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
            Tree<CellBase> result = new HtmlTables().Parse(string.Empty);
            Assert.IsTrue(result.Branches.Count == 0);
        }
    
        [Test] public void ParseNoTables() {
            Tree<CellBase> result = new HtmlTables().Parse("set the table");
            Assert.IsTrue(result.Branches.Count == 0);
            result = new HtmlTables().Parse("set the <table");
            Assert.IsTrue(result.Branches.Count == 0);
        }
    
        [Test, ExpectedException(typeof(ApplicationException))] public void ParseEmptyTable() {
            new HtmlTables().Parse("leader<table x=\"y\"></table>trailer");
        }
    
        [Test, ExpectedException(typeof(ApplicationException))] public void ParseTableWithBody() {
            new HtmlTables().Parse("leader<Table foo=2>body</table>trailer");

        }
    
        [Test] public void ParseTwoTables() {
            string result = Format(new HtmlTables().Parse("<table><tr><td>x</td></tr></table>leader<table><tr><td>x</td></tr></table>trailer"), " ");
            Assert.AreEqual(" <table> <tr> <td> x</td></tr></table> leader <table> <tr> <td> x</td></tr></table> trailer", result);
        }

        [Test, ExpectedException(typeof(ApplicationException))] public void ParseRow() {
            new HtmlTables().Parse(" <table>leader<tr></tr></table>");
        }
    
        [Test] public void ParseCell() {
            string result = Format(new HtmlTables().Parse("<table x=\"y\"><tr><td>content</td></tr></table>"), " ");
            Assert.AreEqual(" <table x=\"y\"> <tr> <td> content</td></tr></table>", result);
        }
    
        [Test] public void ParseRowWithCellAndBody() {
            string result = Format(new HtmlTables().Parse("<table><tr><td>content</td>somebody</tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> content</td> somebody</tr></table>", result);
        }
    
        [Test] public void ParseCellMixedCase() {
            string result = Format(new HtmlTables().Parse("leader<table><TR><Td>body</tD></TR></table>trailer"), " ");
            Assert.AreEqual(" leader <table> <TR> <Td> body</tD></TR></table> trailer", result);
        }
    
        [Test] public void ParseNestedTable() {
            string result = Format(new HtmlTables().Parse("<table><tr><td><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }
    
        [Test] public void ParseTwoNestedTables() {
            string result = Format(new HtmlTables().Parse("<table><tr><td><table><tr><td>content</td></tr></table><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <table> <tr> <td> content</td></tr></table> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }
    
        [Test] public void ParseNestedTableAndBody() {
            string result = Format(new HtmlTables().Parse("<table><tr><td><table><tr><td>content</td></tr></table>somebody</td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <table> <tr> <td> content</td></tr></table> somebody</td></tr></table>", result);
        }
    
        [Test] public void ParseNestedTableAndSplitBody() {
            string result = Format(new HtmlTables().Parse("<table><tr><td>some<table><tr><td>content</td></tr></table>body</td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> some <table> <tr> <td> content</td></tr></table> body</td></tr></table>", result);
        }
    
        [Test] public void ParseNestedList() {
            string result = Format(new HtmlTables().Parse("<table><tr><td><ul><li>content</li></ul></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <ul> <li> content</li></ul></td></tr></table>", result);
        }
    
        [Test] public void ParseNestedListAndTable() {
            string result = Format(new HtmlTables().Parse("<table><tr><td><ul><li>content</li></ul><table><tr><td>content</td></tr></table></td></tr></table>"), " ");
            Assert.AreEqual(" <table> <tr> <td> <ul> <li> content</li></ul> <table> <tr> <td> content</td></tr></table></td></tr></table>", result);
        }

        static string Format(Tree<CellBase> theParseTree, string theSeparator) {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(theParseTree.Value.GetAttribute(CellAttribute.Leader)))
                result.AppendFormat("{0}{1}", theParseTree.Value.GetAttribute(CellAttribute.Leader), theSeparator);
            result.Append(theParseTree.Value.GetAttribute(CellAttribute.StartTag));
            foreach (Tree<CellBase> branch in theParseTree.Branches)
                result.AppendFormat("{0}{1}", theSeparator, Format(branch, theSeparator));
            if (!string.IsNullOrEmpty(theParseTree.Value.GetAttribute(CellAttribute.Body)))
                result.AppendFormat("{0}{1}", theSeparator, theParseTree.Value.GetAttribute(CellAttribute.Body));
            result.Append(theParseTree.Value.GetAttribute(CellAttribute.EndTag));
            if (!string.IsNullOrEmpty(theParseTree.Value.GetAttribute(CellAttribute.Trailer)))
                result.AppendFormat("{0}{1}", theSeparator, theParseTree.Value.GetAttribute(CellAttribute.Trailer));
            return result.ToString();
        }
    }
}