// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using fitSharp.Parser;
using NUnit.Framework;
using System.Linq;
using System.Xml.Linq;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture]
    public class ExcelSheetTest {

        [Test]
        public void ParsesSingleCell() {
            AssertParses(Worksheet("A1"),
                " <table class=\"fit_table\"> <tr> <td> A1value</td></tr></table>");
        }

        [Test]
        public void ParsesSingleRow() {
            AssertParses(Worksheet("A1", "B1"),
                " <table class=\"fit_table\"> <tr> <td> A1value</td> <td> B1value</td></tr></table>");
        }

        [Test]
        public void ParsesSingleRowWithGaps() {
            AssertParses(Worksheet("C1", "A1"),
                " <table class=\"fit_table\"> <tr> <td> A1value</td> <td></td> <td> C1value</td></tr></table>");
        }

        [Test]
        public void ParsesMultipleRows() {
            AssertParses(Worksheet("A1", "A2"),
                " <table class=\"fit_table\"> <tr> <td> A1value</td></tr> <tr> <td> A2value</td></tr></table>");
        }

        [Test]
        public void ParsesMultipleTables() {
            AssertParses(Worksheet("A1", "A3"),
                " <table class=\"fit_table\"> <tr> <td> A1value</td></tr></table> <br /> <table class=\"fit_table\"> <tr> <td> A3value</td></tr></table>");
        }

        [Test]
        public void SubstitutesStrings() {
            AssertParses(
                "<worksheet xmlns=\"myNamespace\"><sheetData><row><c r=\"A1\" t=\"s\"><v>1</v></c><c r=\"B1\" t=\"s\"><v>0</v></c></row></sheetData></worksheet>",
                " <table class=\"fit_table\"> <tr> <td> def</td> <td> abc</td></tr></table>",
                "<sst xmlns=\"myNamespace\"><si><t>abc</t></si><si><t>def</t></si></sst>");
        }

        static void AssertParses(string worksheet, string expected, string strings = "<sst/>") {
            var sheet = new ExcelSheet(text => new TreeList<Cell>(new CellBase(text)));
            sheet.LoadStrings(XDocument.Parse(strings));
            var sheetDocument = XDocument.Parse(worksheet);
            Assert.AreEqual(expected, sheet.Parse(sheetDocument).Format());
        }

        static string Worksheet(params string[] addresses) {
            var result = addresses.Aggregate(
                "<worksheet xmlns=\"myNamespace\"><sheetData>",
                (current, address) => current + ("<row><c r=\"" + address + "\"><v>" + address + "value</v></c></row>"));
            result += "</sheetData></worksheet>";
            return result;
        }
    }
}
