// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public static class TreeCellWriter {
        public static string WriteBranches(this Tree<Cell> input) {
		    var result = new StringBuilder();
            BuildBranches(input, result);
			return result.ToString();
        }

        public static string WriteTree(this Tree<Cell> input) {
		    var result = new StringBuilder();
            BuildCell(result, input);
			return result.ToString();
        }

        public static string Body(Cell cell) {
            var body = new StringBuilder();
            cell.FormatAttribute(CellAttribute.Body, body);
            cell.FormatAttribute(CellAttribute.Syntax, body);
            cell.FormatAttribute(CellAttribute.Formatted, body);
            cell.FormatAttribute(CellAttribute.Add, body);
            cell.FormatAttribute(CellAttribute.InformationPrefix, body);
            cell.FormatAttribute(CellAttribute.InformationSuffix, body);
            cell.FormatAttribute(CellAttribute.Actual, body);
            cell.FormatAttribute(CellAttribute.Exception, body);
            cell.FormatAttribute(CellAttribute.Label, body);
            cell.FormatAttribute(CellAttribute.Folded, body);
            return body.ToString();
        }

        static void BuildBranches(Tree<Cell> cells, StringBuilder builder) {
            foreach(var child in cells.Branches) {
                BuildCell(builder, child);
            }
        }

		static void BuildCell(StringBuilder builder, Tree<Cell> cells) {
		    var cell = cells.Value;
		    if (cell != null) {
		        cell.FormatAttribute(CellAttribute.Leader, builder);
		        builder.Append(Tag(cell));
		    }
		    BuildBranches(cells, builder);
		    if (cell != null) {
		        builder.Append(Body(cell));
		        cell.FormatAttribute(CellAttribute.EndTag, builder);
		        cell.FormatAttribute(CellAttribute.Trailer, builder);
		    }
		}

        static string Tag(Cell cell) {
            var tag = new StringBuilder();
            cell.FormatAttribute(CellAttribute.StartTag, tag);
            cell.FormatAttribute(CellAttribute.Status, tag);
            cell.FormatAttribute(CellAttribute.Title, tag);
            return tag.ToString();
        }
    }
}
