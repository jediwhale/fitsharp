// Copyright � 2016 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Machine.Model;
using fitSharp.Parser;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ParseTest
    {

        [Test]
        public void UnEscapeShouldRemoveHtmlEscapes() 
        {
            ClassicAssert.AreEqual("a<b", new HtmlString("a&lt;b").ToPlainText());
            ClassicAssert.AreEqual("a>b & b>c &&", new HtmlString("a&gt;b&nbsp;&amp;&nbsp;b>c &&").ToPlainText());
            ClassicAssert.AreEqual("&amp;&amp;", new HtmlString("&amp;amp;&amp;amp;").ToPlainText());
            ClassicAssert.AreEqual("a>b & b>c &&", new HtmlString("a&gt;b&nbsp;&amp;&nbsp;b>c &&").ToPlainText());
        }

        [Test]
        public void UnFormatShouldRemoveHtmlFormattingCodeIfPresent() 
        {
            ClassicAssert.AreEqual("ab",new HtmlString("<font size=+1>a</font>b").ToPlainText());
            ClassicAssert.AreEqual("ab",new HtmlString("a<font size=+1>b</font>").ToPlainText());
            ClassicAssert.AreEqual("a<b",new HtmlString("a<b").ToPlainText());
        }

        [Test]
        public void LeaderShouldReturnAllHtmlTextBeforeTheParse()
        {
            var p = Parse.ParseFrom("<html><head></head><body><Table foo=2><tr><td>body</td></tr></table></body></html>");
            ClassicAssert.AreEqual("<html><head></head><body>", p.Leader);
            ClassicAssert.AreEqual("<Table foo=2>", p.Tag);
            ClassicAssert.AreEqual("</body></html>", p.Trailer);
        }

        static Parse SimpleTableParse
        {
            get { return Parse.ParseFrom("leader<table><tr><td>body</td></tr></table>trailer"); }
        }

        [Test]
        public void BodyShouldReturnNullForTables() 
        {
            Parse parse = SimpleTableParse;
            ClassicAssert.IsTrue(string.IsNullOrEmpty(parse.Body));
        }

        [Test]
        public void BodyShouldReturnNullForRows() 
        {
            Parse parse = SimpleTableParse;
            ClassicAssert.IsTrue(string.IsNullOrEmpty(parse.Parts.Body));
        }

        [Test]
        public void BodyShouldReturnTextForCells() 
        {
            Parse parse = SimpleTableParse;
            ClassicAssert.AreEqual("body", parse.Parts.Parts.Body);
        }

        [Test]
        public void PartsShouldReturnCellsWhenTheParseRepresentsARow()
        {
            Parse row = Parse.ParseFrom("<table><tr><td>one</td><td>two</td><td>three</td></tr></table>").Parts;
            ClassicAssert.AreEqual("one", row.Parts.Body);
            ClassicAssert.AreEqual("two", row.Parts.More.Body);
            ClassicAssert.AreEqual("three", row.Parts.More.More.Body);
        }

        [Test]
        public void PartsShouldReturnRowsWhenTheParseRepresentsATable()
        {
            var table = Parse.ParseFrom("<table><tr><td>row one</td></tr><tr><td>row two</td></tr></table>");
            ClassicAssert.AreEqual("row one", table.Parts.Parts.Body);
            ClassicAssert.AreEqual("row two", table.Parts.More.Parts.Body);
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
            ClassicAssert.AreEqual("one", p.At(0,0,0).Body);
            ClassicAssert.AreEqual("two", p.At(0,0,1).Body);
            ClassicAssert.AreEqual("three", p.At(0,0,2).Body);
            ClassicAssert.AreEqual("three", p.At(0,0,3).Body);
            ClassicAssert.AreEqual("three", p.At(0,0,4).Body);
            ClassicAssert.AreEqual("four", p.At(0,1,0).Body);
            ClassicAssert.AreEqual("four", p.At(0,1,1).Body);
            ClassicAssert.AreEqual("four", p.At(0,2,0).Body);
            ClassicAssert.AreEqual(1, p.Size);
            ClassicAssert.AreEqual(2, p.Parts.Size);
            ClassicAssert.AreEqual(3, p.Parts.Parts.Size);
            ClassicAssert.AreEqual("one", p.Leaf.Body);
            ClassicAssert.AreEqual("four", p.Parts.Last.Leaf.Body);
        }

        [Test]
        public void TestParseException() 
        {
            try 
            {
                Parse.ParseFrom("leader<table><tr><th>one</th><th>two</th><th>three</th></tr><tr><td>four</td></tr></table>trailer");
                ClassicAssert.Fail("expected Exception not thrown");
            } 
            catch (ApplicationException e) 
            {
                ClassicAssert.AreEqual("Can't find tag: td", e.Message);
            }
        }

        [Test]
        public void TestText() {
            Parse p = Parse.ParseFrom("<table><tr><td>a&lt;b</td></tr></table>").Parts.Parts;
            ClassicAssert.AreEqual("a&lt;b", p.Body);
            ClassicAssert.AreEqual("a<b", p.Text);
            p = Parse.ParseFrom("<table><tr><td>\ta&gt;b&nbsp;&amp;&nbsp;b>c &&&nbsp;</td></tr></table>").Parts.Parts;
            ClassicAssert.AreEqual("\ta>b & b>c && ", p.Text);
            p = Parse.ParseFrom("<table><tr><td>\ta&gt;b&nbsp;&amp;&nbsp;b>c &&nbsp;</td></tr></table>").Parts.Parts;
            ClassicAssert.AreEqual("\ta>b & b>c & ", p.Text);
            p = Parse.ParseFrom("<table><tr><TD><P><FONT FACE=\"Arial\" SIZE=2>GroupTestFixture</FONT></TD></tr></table>").Parts.Parts;
            ClassicAssert.AreEqual("GroupTestFixture",p.Text);
        }

        [Test]
        public void TestContent() {
            Parse p = Parse.ParseFrom("<table><tr><td>\r\n\t a \r\n\t</td></tr></table>").Parts.Parts;
            ClassicAssert.AreEqual("a", p.Content);

            p = Parse.ParseFrom("<table><tr><td>\r\n\t a \r\n\tb</td></tr></table>").Parts.Parts;
            ClassicAssert.AreEqual("a \r\n\tb", p.Content);
        }

        [Test] public void CopyFromLeaf() {
            var source = MakeRootNode();
            source.Value.SetAttribute(CellAttribute.Body, "body");
            AssertCopyFromResult(source, "leader<tag stuff>body</tag>trailer");
        }

        static void AssertCopyFromResult(Tree<Cell> source, string expected) {
            ClassicAssert.AreEqual(expected, Parse.CopyFrom(source).ToString());
        }

        static TreeList<Cell> MakeRootNode() {
            var sourceNode = new CellBase("text");
            sourceNode.SetAttribute(CellAttribute.EndTag, "</tag>");
            sourceNode.SetAttribute(CellAttribute.Leader, "leader");
            sourceNode.SetAttribute(CellAttribute.StartTag, "<tag stuff>");
            sourceNode.SetAttribute(CellAttribute.Trailer, "trailer");
            return new TreeList<Cell>(sourceNode);
        }

        [Test] public void CopyFromWithBranch() {
            var source = MakeRootNode();
            AddBranch(source, "leaf");
            AssertCopyFromResult(source, "leader<tag stuff><leaftag>leaf</leaftag></tag>trailer");
        }

        static void AddBranch(TreeList<Cell> source, string body) {
            var branchNode = new CellBase("text");
            branchNode.SetAttribute(CellAttribute.Body, body);
            branchNode.SetAttribute(CellAttribute.EndTag, "</leaftag>");
            branchNode.SetAttribute(CellAttribute.StartTag, "<leaftag>");
            source.AddBranchValue(branchNode);
        }

        [Test] public void CopyFromWithBranches() {
            var source = MakeRootNode();
            for (int i = 0; i < 2; i++) AddBranch(source, "leaf" + i);
            AssertCopyFromResult(source, "leader<tag stuff><leaftag>leaf0</leaftag><leaftag>leaf1</leaftag></tag>trailer");
        }

        [Test] public void BranchesAreAdded() {
            var parse = new Parse("tr", string.Empty, null, null);
            parse.Add(new Parse("td", "first", null, null));
            ClassicAssert.AreEqual("first", parse.Parts.Body);
            parse.Add(new Parse("td", "second", null, null));
            ClassicAssert.AreEqual("second", parse.Parts.More.Body);
            parse.Add(new Parse("td", "third", null, null));
            ClassicAssert.AreEqual("third", parse.Parts.Last.Body);
        }
    }
}
