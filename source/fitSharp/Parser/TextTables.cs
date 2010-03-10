// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Parser {
    public class TextTables {
        TextTableScanner scanner;

        public Tree<CellBase> Parse(string input) {
            scanner = new TextTableScanner(input);
            var result = new TreeList<CellBase>(new CellBase(string.Empty));
            MakeTables(result);
            return result;
        }

        void MakeTables(TreeList<CellBase> result) {
            string leader = string.Empty;
            TreeList<CellBase> table = null;
            scanner.MoveNext();
            do {
                if (scanner.Current.Type == TokenType.Leader) {
                    leader = scanner.Current.Content;
                    scanner.MoveNext();
                }
                else if (scanner.Current.Type == TokenType.Word) {
                    table = new TreeList<CellBase>(new CellBase(string.Empty));
                    table.Value.SetAttribute(CellAttribute.StartTag, "<p>");
                    table.Value.SetAttribute(CellAttribute.EndTag, "</p>");
                    if (leader.Length > 0) {
                        table.Value.SetAttribute(CellAttribute.Leader, leader);
                        leader = string.Empty;
                    }
                    result.AddBranch(table);
                    MakeRows(table);
                    if (scanner.Current.Type == TokenType.Newline) scanner.MoveNext();
                }
                else {
                    scanner.MoveNext();
                }
            } while (scanner.Current.Type != TokenType.End);
        
            if (table != null && scanner.Current.Content.Length > 0) {
                 table.Value.SetAttribute(CellAttribute.Trailer, scanner.Current.Content);
            }
        }

        void MakeRows(TreeList<CellBase> table) {
            do {
                var row = new TreeList<CellBase>(new CellBase(string.Empty));
                row.Value.SetAttribute(CellAttribute.StartTag, "<div>");
                row.Value.SetAttribute(CellAttribute.EndTag, "</div>");
                table.AddBranch(row);
                MakeCells(row);
                if (scanner.Current.Type == TokenType.Newline) scanner.MoveNext();
            } while (scanner.Current.Type == TokenType.Word);
        }

        void MakeCells(TreeList<CellBase> row) {
            while (scanner.Current.Type == TokenType.Word) {
                var cell = new TreeList<CellBase>(new CellBase(scanner.Current.Content));
                cell.Value.SetAttribute(CellAttribute.Body, scanner.Current.Content);
                cell.Value.SetAttribute(CellAttribute.StartTag, "<span>");
                cell.Value.SetAttribute(CellAttribute.EndTag, "</span> ");
                row.AddBranch(cell);
                scanner.MoveNext();
            }
        }
    }
}
