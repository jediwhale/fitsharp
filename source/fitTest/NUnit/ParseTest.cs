// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Machine.Model;
using fitSharp.Parser;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ParseTest
    {

        [Test]
        public void UnEscapeShouldRemoveHtmlEscapes() 
        {
            Assert.AreEqual("a<b", new HtmlString("a&lt;b").ToPlainText());
            Assert.AreEqual("a>b & b>c &&", new HtmlString("a&gt;b&nbsp;&amp;&nbsp;b>c &&").ToPlainText());
            Assert.AreEqual("&amp;&amp;", new HtmlString("&amp;amp;&amp;amp;").ToPlainText());
            Assert.AreEqual("a>b & b>c &&", new HtmlString("a&gt;b&nbsp;&amp;&nbsp;b>c &&").ToPlainText());
        }

        [Test]
        public void UnFormatShouldRemoveHtmlFormattingCodeIfPresent() 
        {
            Assert.AreEqual("ab",new HtmlString("<font size=+1>a</font>b").ToPlainText());
            Assert.AreEqual("ab",new HtmlString("a<font size=+1>b</font>").ToPlainText());
            Assert.AreEqual("a<b",new HtmlString("a<b").ToPlainText());
        }

        [Test]
        public void LeaderShouldReturnAllHtmlTextBeforeTheParse()
        {
            var p = Parse.ParseFrom("<html><head></head><body><Table foo=2><tr><td>body</td></tr></table></body></html>");
            Assert.AreEqual("<html><head></head><body>", p.Leader);
            Assert.AreEqual("<Table foo=2>", p.Tag);
            Assert.AreEqual("</body></html>", p.Trailer);
        }

        static Parse SimpleTableParse
        {
            get { return Parse.ParseFrom("leader<table><tr><td>body</td></tr></table>trailer"); }
        }

        [Test]
        public void BodyShouldReturnNullForTables() 
        {
            Parse parse = SimpleTableParse;
            Assert.IsTrue(string.IsNullOrEmpty(parse.Body));
        }

        [Test]
        public void BodyShouldReturnNullForRows() 
        {
            Parse parse = SimpleTableParse;
            Assert.IsTrue(string.IsNullOrEmpty(parse.Parts.Body));
        }

        [Test]
        public void BodyShouldReturnTextForCells() 
        {
            Parse parse = SimpleTableParse;
            Assert.AreEqual("body", parse.Parts.Parts.Body);
        }

        [Test]
        public void PartsShouldReturnCellsWhenTheParseRepresentsARow()
        {
            Parse row = Parse.ParseFrom("<table><tr><td>one</td><td>two</td><td>three</td></tr></table>").Parts;
            Assert.AreEqual("one", row.Parts.Body);
            Assert.AreEqual("two", row.Parts.More.Body);
            Assert.AreEqual("three", row.Parts.More.More.Body);
        }

        [Test]
        public void PartsShouldReturnRowsWhenTheParseRepresentsATable()
        {
            var table = Parse.ParseFrom("<table><tr><td>row one</td></tr><tr><td>row two</td></tr></table>");
            Assert.AreEqual("row one", table.Parts.Parts.Body);
            Assert.AreEqual("row two", table.Parts.More.Parts.Body);
        }

        [Test]
        public void TestIndexingPage() 
        {
            var p = Parse.ParseFrom(
                @"leader
					<table>
						<tr>
							<td>one</td><td>two</td><td>three</td>
						</tr>
						<tr>
							<td>four</td>
						</tr>
					</table>
				trailer");
            Assert.AreEqual("one", p.At(0,0,0).Body);
            Assert.AreEqual("two", p.At(0,0,1).Body);
            Assert.AreEqual("three", p.At(0,0,2).Body);
            Assert.AreEqual("three", p.At(0,0,3).Body);
            Assert.AreEqual("three", p.At(0,0,4).Body);
            Assert.AreEqual("four", p.At(0,1,0).Body);
            Assert.AreEqual("four", p.At(0,1,1).Body);
            Assert.AreEqual("four", p.At(0,2,0).Body);
            Assert.AreEqual(1, p.Size);
            Assert.AreEqual(2, p.Parts.Size);
            Assert.AreEqual(3, p.Parts.Parts.Size);
            Assert.AreEqual("one", p.Leaf.Body);
            Assert.AreEqual("four", p.Parts.Last.Leaf.Body);
        }

        [Test]
        public void TestParseException() 
        {
            try 
            {
                Parse.ParseFrom("leader<table><tr><th>one</th><th>two</th><th>three</th></tr><tr><td>four</td></tr></table>trailer");
                Assert.Fail("expected Exception not thrown");
            } 
            catch (ApplicationException e) 
            {
                Assert.AreEqual("Can't find tag: td", e.Message);
                return;
            }
        }

        [Test]
        public void TestText() 
        {
            Parse p = Parse.ParseFrom("<table><tr><td>a&lt;b</td></tr></table>").Parts.Parts;
            Assert.AreEqual("a&lt;b", p.Body);
            Assert.AreEqual("a<b", p.Text);
            p = Parse.ParseFrom("<table><tr><td>\ta&gt;b&nbsp;&amp;&nbsp;b>c &&&nbsp;</td></tr></table>").Parts.Parts;
            Assert.AreEqual("\ta>b & b>c && ", p.Text);
            p = Parse.ParseFrom("<table><tr><td>\ta&gt;b&nbsp;&amp;&nbsp;b>c &&nbsp;</td></tr></table>").Parts.Parts;
            Assert.AreEqual("\ta>b & b>c & ", p.Text);
            p = Parse.ParseFrom("<table><tr><TD><P><FONT FACE=\"Arial\" SIZE=2>GroupTestFixture</FONT></TD></tr></table>").Parts.Parts;
            Assert.AreEqual("GroupTestFixture",p.Text);
        }

        [Test] public void CopyFromLeaf() {
            TreeList<CellBase> source = MakeRootNode();
            source.Value.SetAttribute(CellAttribute.Body, "body");
            AssertCopyFromResult(source, "leader<tag stuff>body</tag>trailer");
        }

        static void AssertCopyFromResult(Tree<CellBase> source, string expected) {
            Assert.AreEqual(expected, Parse.CopyFrom(source).ToString());
        }

        static TreeList<CellBase> MakeRootNode() {
            var sourceNode = new CellBase("text");
            sourceNode.SetAttribute(CellAttribute.EndTag, "</tag>");
            sourceNode.SetAttribute(CellAttribute.Leader, "leader");
            sourceNode.SetAttribute(CellAttribute.StartTag, "<tag stuff>");
            sourceNode.SetAttribute(CellAttribute.Trailer, "trailer");
            return new TreeList<CellBase>(sourceNode);
        }

        [Test] public void CopyFromWithBranch() {
            TreeList<CellBase> source = MakeRootNode();
            AddBranch(source, "leaf");
            AssertCopyFromResult(source, "leader<tag stuff><leaftag>leaf</leaftag></tag>trailer");
        }

        static void AddBranch(TreeList<CellBase> source, string body) {
            var branchNode = new CellBase("text");
            branchNode.SetAttribute(CellAttribute.Body, body);
            branchNode.SetAttribute(CellAttribute.EndTag, "</leaftag>");
            branchNode.SetAttribute(CellAttribute.StartTag, "<leaftag>");
            source.AddBranchValue(branchNode);
        }

        [Test] public void CopyFromWithBranches() {
            TreeList<CellBase> source = MakeRootNode();
            for (int i = 0; i < 2; i++) AddBranch(source, "leaf" + i);
            AssertCopyFromResult(source, "leader<tag stuff><leaftag>leaf0</leaftag><leaftag>leaf1</leaftag></tag>trailer");
        }
    }
}
