// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using System.Web;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseStoryTestString: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(StoryTestString);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return new TypedValue(new StoryTestString(ToString(parameters)));
        }

		string ToString(Tree<Cell> cells) {
		    var result = new StringBuilder();
            BuildBranches(cells, result);
			return result.ToString();
		}

        void BuildBranches(Tree<Cell> cells, StringBuilder builder) {
            foreach(Tree<Cell> child in cells.Branches) {
                BuildCell(builder, child);
            }
        }

		void BuildCell(StringBuilder builder, Tree<Cell> cells) {
		    Cell cell = cells.Value;
			builder.Append(cell.GetAttribute(CellAttribute.Leader));
			builder.Append(Tag(cell));
            BuildBranches(cells, builder);
            builder.Append(Body(cell));
			builder.Append(cell.GetAttribute(CellAttribute.EndTag));
			builder.Append(cell.GetAttribute(CellAttribute.Trailer));
		}

        string Tag(Cell cell) {
	        string tag = cell.GetAttribute(CellAttribute.StartTag);
	        int space = tag.IndexOf(' ');
	        if (space < 0) space = tag.Length - 1;
	        return !cell.HasAttribute(CellAttribute.Status)
                    ? tag
                    : string.Format("{0} class=\"{1}\"{2}", tag.Substring(0, space), cell.GetAttribute(CellAttribute.Status), tag.Substring(space));
        }

        public static string Body(Cell cell) {
            string result = cell.GetAttribute(CellAttribute.Body);
            if (cell.HasAttribute(CellAttribute.Add)) {
                if (cell.HasAttribute(CellAttribute.Formatted)) {
                    result = string.Format("<pre>{0}</pre>", HttpUtility.HtmlEncode(result));
                }
                else if (!cell.HasAttribute(CellAttribute.Raw)) {
                    result = string.Format("<span class=\"fit_grey\">{0}</span>", HttpUtility.HtmlEncode(result));
                }
            }
            if (cell.HasAttribute(CellAttribute.InformationPrefix)) {
                result = string.Format("<span class=\"fit_grey\">{0}</span>{1}", cell.GetAttribute(CellAttribute.InformationPrefix), result);
            }
            if (cell.HasAttribute(CellAttribute.InformationSuffix)) {
                result = string.Format("{0}<span class=\"fit_grey\">{1}</span>", result, cell.GetAttribute(CellAttribute.InformationSuffix));
            }
            if (cell.HasAttribute(CellAttribute.Actual)) {
                result += Label("expected") + "<hr />" + HttpUtility.HtmlEncode(cell.GetAttribute(CellAttribute.Actual)) + Label("actual");
            }
            if (cell.HasAttribute(CellAttribute.Exception)) {
                result += "<hr /><pre><div class=\"fit_stacktrace\">" + cell.GetAttribute(CellAttribute.Exception) + "</div></pre>";
            }
            if (cell.HasAttribute(CellAttribute.Label)) {
                result += Label(cell.GetAttribute(CellAttribute.Label));
            }
            if (cell.HasAttribute(CellAttribute.Folded)) {
                var foldedText = cell.GetAttribute(CellAttribute.Folded);
                if (string.IsNullOrEmpty(foldedText)) {
                    result = Folded(result);
                }
                else {
                    result += Folded(foldedText);
                }
            }
            return result;
        }

        static string Folded(string text) {
            return string.Format(
                "<span><a href=\"javascript:void(0)\" onclick=\"this.parentNode.nextSibling.style.display="
                + "this.parentNode.nextSibling.style.display=='none'?'':'none'\">&#8659;</a></span><div style=\"display:none\"><div class=\"fit_extension\">{0}</div></div>",
                text);
        }

	    static string Label(string text) {
			return " <span class=\"fit_label\">" + text + "</span>";
		}
    }
}
