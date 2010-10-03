// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Web;
using fitSharp.Machine.Model;

namespace fitSharp.Parser {
    public class TextTables {
        readonly TextTableScanner scanner;
        string[] startTags;
        string[] endTags;

        public TextTables(TextTableScanner scanner) {
            this.scanner = scanner;
        }

        public Tree<CellBase> Parse() {
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
                    SetTags();
                    table = new TreeList<CellBase>(new CellBase(string.Empty));
                    table.Value.SetAttribute(CellAttribute.StartTag, startTags[0]);
                    table.Value.SetAttribute(CellAttribute.EndTag, endTags[0]);
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

        void SetTags() {
            if (scanner.StartOfLine == CharacterType.Separator) {
                startTags = new [] {"<table>", "<tr>", "<td>"};
                endTags = new [] {"</table>", "</tr>", "</td> "};
            }
            else {
                startTags = new [] {"<p>", "<table><tr>", "<td>"};
                endTags = new [] {"</p>", "</tr></table>", "</td> "};
            }
        }

        void MakeRows(TreeList<CellBase> table) {
            do {
                var row = new TreeList<CellBase>(new CellBase(string.Empty));
                row.Value.SetAttribute(CellAttribute.StartTag, startTags[1]);
                row.Value.SetAttribute(CellAttribute.EndTag, endTags[1]);
                table.AddBranch(row);
                MakeCells(row);
                if (scanner.Current.Type == TokenType.Newline) scanner.MoveNext();
            } while (scanner.Current.Type == TokenType.Word);
        }

        void MakeCells(TreeList<CellBase> row) {
            while (scanner.Current.Type == TokenType.Word) {
                var cell = new TreeList<CellBase>(new CellBase(scanner.Current.Content));
                cell.Value.SetAttribute(CellAttribute.Body, HttpUtility.HtmlEncode(scanner.Current.Content));
                cell.Value.SetAttribute(CellAttribute.StartTag, startTags[2]);
                cell.Value.SetAttribute(CellAttribute.EndTag, endTags[2]);
                row.AddBranch(cell);
                scanner.MoveNext();
            }
        }
    }
}
