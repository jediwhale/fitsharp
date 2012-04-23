// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseStoryTestString: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(StoryTestString) || type == typeof(StoryTableString);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(StoryTestString)
                ? new TypedValue(new StoryTestString(TestToString(parameters)))
                : new TypedValue(new StoryTableString(TableToString(parameters)));
        }

        string TableToString(Tree<Cell> cells) {
		    var result = new StringBuilder();
            BuildCell(result, cells);
			return result.ToString();
		}

        string TestToString(Tree<Cell> cells) {
		    var result = new StringBuilder();
            BuildBranches(cells, result);
			return result.ToString();
		}

        void BuildBranches(Tree<Cell> cells, StringBuilder builder) {
            foreach(var child in cells.Branches) {
                BuildCell(builder, child);
            }
        }

		void BuildCell(StringBuilder builder, Tree<Cell> cells) {
		    var cell = cells.Value;
            cell.FormatAttribute(CellAttribute.Leader, builder);
			builder.Append(Tag(cell));
            BuildBranches(cells, builder);
            builder.Append(Body(cell));
            cell.FormatAttribute(CellAttribute.EndTag, builder);
            cell.FormatAttribute(CellAttribute.Trailer, builder);
		}

        static string Tag(Cell cell) {
            var tag = new StringBuilder();
            cell.FormatAttribute(CellAttribute.StartTag, tag);
            cell.FormatAttribute(CellAttribute.Status, tag);
            return tag.ToString();
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
    }
}
