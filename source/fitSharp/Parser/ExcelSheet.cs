// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace fitSharp.Parser {
    public class ExcelSheet {

        public ExcelSheet(Func<string, Tree<Cell>> makeTreeCell) {
            this.makeTreeCell = makeTreeCell;
        }

        public Tree<Cell> Parse(XDocument worksheet) {
            var currentRow = int.MinValue;
            var result = makeTreeCell(string.Empty);
            foreach (var cell in MakeCells(worksheet).OrderBy(c => c.Row).ThenBy(c => c.Column)) {
                if (cell.Row != currentRow) {
                    if (cell.Row > currentRow + 1) {
                        result.Add(MakeTable(result.Branches.Count));
                    }
                    result.Branches.Last().Add(MakeRow());
                    currentRow = cell.Row;
                }
                var row = result.Branches.Last().Branches.Last();
                while (row.Branches.Count < cell.Column) row.Add(MakeCell(string.Empty));
                row.Add(MakeCell(cell.Content));
            }
            return result;
        }

        public void LoadStrings(XDocument strings) {
            sharedStrings.Clear();
            var nameSpace = strings.Root.Name.Namespace;
            sharedStrings.AddRange(strings.Root.Elements(nameSpace + "si").Select(e => e.Element(nameSpace + "t").Value));
        }

        IEnumerable<ExcelCell> MakeCells(XDocument worksheet) {
            var nameSpace = worksheet.Root.Name.Namespace;
            var cells = new List<ExcelCell>();
            cells.AddRange(
                worksheet.Root.Elements(nameSpace + "sheetData")
                    .SelectMany(element => element.Descendants(nameSpace + "c"))
                    .Select(column => MakeCell(column, nameSpace)));
            return cells;
        }

        ExcelCell MakeCell(XElement column, XNamespace nameSpace) {
            var content = column.Elements(nameSpace + "v").First().Value;
            if (column.Attributes("t").Any(attribute => attribute.Value == "s")) {
                content = sharedStrings[int.Parse(content)];
            }
            return new ExcelCell(column.Attributes("r").First().Value, content);
        }

        Tree<Cell> MakeTable(int existingTableCount) {
            var table = makeTreeCell(string.Empty);
            table.Value.SetAttribute(CellAttribute.StartTag, "<table class=\"fit_table\">");
            table.Value.SetAttribute(CellAttribute.EndTag, "</table>");
            if (existingTableCount > 0) {
                table.Value.SetAttribute(CellAttribute.Leader, "<br />");
            }
            return table;
        }

        Tree<Cell> MakeRow() {
            var row = makeTreeCell(string.Empty);
            row.Value.SetTag("tr");
            return row;
        }

        Tree<Cell> MakeCell(string content) {
            var cell = makeTreeCell(content);
            cell.Value.SetTag("td");
            cell.Value.SetAttribute(CellAttribute.Body, HttpUtility.HtmlEncode(content));
            return cell;
        }

        readonly List<string> sharedStrings = new List<string>();
        readonly Func<string, Tree<Cell>> makeTreeCell;
    }
}
