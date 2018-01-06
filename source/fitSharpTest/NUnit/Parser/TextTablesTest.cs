// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Machine.Model;
using fitSharp.Parser;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture] public class TextTablesTest {
        [Test] public void ParsesEmptyInputAsEmptyTree() {
            Assert.AreEqual(0, Parse(string.Empty).Branches.Count);
        }

        [Test] public void ParsesWordAsCell() {
            AssertParse("stuff", " <div> <table><tr> <td> stuff</td> </tr></table></div>");
        }

        [Test] public void GeneratesTableForLineStartingWithSeparator() {
            AssertParse("\n|stuff", "<br /> <table class=\"fit_table\"> <tr> <td> stuff</td> </tr></table>");
        }

        [Test] public void EncodesCellContent() {
            AssertParse("<<stuff", " <div> <table><tr> <td> &lt;&lt;stuff</td> </tr></table></div>");
        }

        [Test] public void BackSlashesInCellContent() {
            AssertParse("'stuff\\'\\\\morestuff'", " <div> <table><tr> <td> stuff&#39;\\morestuff</td> </tr></table></div>");
        }

        [Test] public void HandlesLeadingNewlines() {
            AssertParse("\n\n\nstuff", "<br /><br /><br /> <div> <table><tr> <td> stuff</td> </tr></table></div>");
        }

        [Test] public void ParsesWordsAsSeparateCells() {
            AssertParse("more|stuff",
                " <div> <table><tr> <td> more</td>  <td> stuff</td> </tr></table></div>");
        }

        [Test] public void ParsesQuotedWordAsSingleCell() {
            AssertParse("'more stuff'",
                " <div> <table><tr> <td> more stuff</td> </tr></table></div>");
        }

        [Test] public void RemovesUnderscores() {
            AssertParse("more_stuff",
                " <div> <table><tr> <td> more stuff</td> </tr></table></div>");
        }

        [Test] public void KeepsQuotedUnderscores() {
            AssertParse("'more_stuff'",
                " <div> <table><tr> <td> more_stuff</td> </tr></table></div>");
        }

        [Test] public void ParsesLinesAsSeparateRows() {
            AssertParse("more 'good'\nstuff",
                " <div> <table><tr> <td> more</td>  <td> good</td> </tr></table> <table><tr> <td> stuff</td> </tr></table></div>");
        }

        [Test] public void ParsesNestedTable() {
            AssertParse("one [\n two\n] three", " <div> <table><tr> <td> one</td>  <td> <div> <table><tr> <td> two</td> </tr></table></div></td>  <td> three</td> </tr></table></div>");
        }

        [Test] public void ParsesNestedTableInFirstCell() {
            AssertParse("one\n[\n two\n]\nthree", " <div> <table><tr> <td> one</td> </tr></table> <table><tr> <td> <div> <table><tr> <td> two</td> </tr></table></div></td> </tr></table> <table><tr> <td> three</td> </tr></table></div>");
        }

        [Test] public void ParsesBlankLinesAsSeparateTables() {
            AssertParse("more\ngood\n\nstuff",
                " <div> <table><tr> <td> more</td> </tr></table> <table><tr> <td> good</td> </tr></table></div> <br /> <div> <table><tr> <td> stuff</td> </tr></table></div>");
        }

        [Test] public void IncludesLeaderInFirstTable() {
            AssertParseRaw("more test@stuff",
                " more test@ <div> <table><tr> <td> stuff</td> </tr></table></div>");
        }

        [Test] public void IncludesTrailerInLastTable() {
            AssertParseRaw("test@stuff@test",
                " test@ <div> <table><tr> <td> stuff</td> </tr></table></div> @test");
        }

        [Test] public void IncludesLeaderInSecondTable() {
            AssertParseRaw("test@stuff@test and test@more",
                " test@ <div> <table><tr> <td> stuff</td> </tr></table></div> @test and test@ <div> <table><tr> <td> more</td> </tr></table></div>");
        }

        static void AssertParse(string input, string expected) {
            Assert.AreEqual(
                " test@" + expected,
                Parse(input).Format());
        }

        static void AssertParseRaw(string input, string expected) {
            Assert.AreEqual(
                expected,
                ParseRaw(input).Format());
        }

        static Tree<Cell> Parse(string input) {
            return ParseRaw("test@" + input);
        }

        static Tree<Cell> ParseRaw(string input) {
            return new TextTables(
                    new TextTableScanner(input, c => c == CharacterType.Letter),
                    text => new TreeList<Cell>(new CellBase(text)))
                .Parse();
        }
    }
}
