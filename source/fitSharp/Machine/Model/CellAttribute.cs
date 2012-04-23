// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace fitSharp.Machine.Model {
    public enum CellAttribute {
        Actual,
        Add,
        Body,
        Difference,
        EndTag,
        Exception,
        Folded,
        Formatted,
        InformationPrefix,
        InformationSuffix,
        Label,
        Leader,
        Raw,
        StartTag,
        Status,
        Syntax,
        Trailer
    }

    public class CellAttributeValue {
        public const string SyntaxInterpreter = "interpreter";
        public const string SyntaxKeyword = "keyword";
        public const string SyntaxMember = "member";
        public const string SyntaxSUT = "SUT";

        public static CellAttributeValue Make(CellAttribute attribute) {
            return factories.ContainsKey(attribute)
                ? factories[attribute](attribute)
                : new CellAttributeValue();
        }

        static readonly Dictionary<CellAttribute, Func<CellAttribute, CellAttributeValue>> factories
            = new Dictionary<CellAttribute, Func<CellAttribute, CellAttributeValue>> {
                {CellAttribute.Add, key => new AddValue()},
                {CellAttribute.Actual, key => new ActualValue()},
                {CellAttribute.Difference, key => new DifferenceValue()},
                {CellAttribute.Exception, key => new ExceptionValue()},
                {CellAttribute.Folded, key => new FoldedValue()},
                {CellAttribute.Formatted, key => new FormattedValue()},
                {CellAttribute.InformationPrefix, key => new InformationPrefixValue()},
                {CellAttribute.InformationSuffix, key => new InformationSuffixValue()},
                {CellAttribute.Label, key => new LabelValue()},
                {CellAttribute.Status, key => new StatusValue()},
                {CellAttribute.Syntax, key => new SyntaxValue()}
        };

        public string Value { get; protected set; }
        public virtual void SetValue(string value) { Value = value; }
        public virtual void Format(Cell cell, StringBuilder input) { input.Append(Value); }

	    protected static string Label(string text) {
			return " <span class=\"fit_label\">" + text + "</span>";
		}

        class ActualValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                input.Append(Label("expected") + "<hr />" + HttpUtility.HtmlEncode(Value) + Label("actual"));
                cell.FormatAttribute(CellAttribute.Difference, input);
            }
        }

        class AddValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                if (cell.HasAttribute(CellAttribute.Formatted) || cell.HasAttribute(CellAttribute.Raw)) return;
                var encodedInput = HttpUtility.HtmlEncode(input.ToString());
                input.Clear();
                input.AppendFormat("<span class=\"fit_grey\">{0}</span>", encodedInput);
            }
        }

        class DifferenceValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                input.Append("<hr />" + HttpUtility.HtmlEncode(Value));
            }
        }

        class ExceptionValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                input.Append("<hr /><pre><div class=\"fit_stacktrace\">" + Value + "</div></pre>");
            }
        }

        class FoldedValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                if (string.IsNullOrEmpty(Value)) {
                    var originalInput = input.ToString();
                    input.Clear();
                    input.Append(Folded(originalInput));
                }
                else {
                    input.Append(Folded(Value));
                }
            }

            static string Folded(string text) {
                return string.Format(
                    "<span><a href=\"javascript:void(0)\" onclick=\"this.parentNode.nextSibling.style.display="
                    + "this.parentNode.nextSibling.style.display=='none'?'':'none'\">&#8659;</a></span>"
                    + "<div style=\"display:none\"><div class=\"fit_extension\">{0}</div></div>",
                    text);
            }
        }

        class FormattedValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                var encodedInput = HttpUtility.HtmlEncode(input.ToString());
                input.Clear();
                input.AppendFormat("<pre>{0}</pre>", encodedInput);
            }
        }

        class InformationPrefixValue: CellAttributeValue {
            public override void SetValue(string value) {
                Value = value + " " + Value ?? string.Empty;
            }

            public override void Format(Cell cell, StringBuilder input) {
                input.Insert(0, string.Format("<span class=\"fit_grey\">{0}</span>", Value));
            }
        }

        class InformationSuffixValue: CellAttributeValue {
            public override void SetValue(string value) {
                Value = Value ?? string.Empty + " " + value;
            }

            public override void Format(Cell cell, StringBuilder input) {
                input.AppendFormat("<span class=\"fit_grey\">{0}</span>", Value);
            }
        }

        class LabelValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                input.Append(Label(Value));
            }
        }

        class StatusValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
	            var space = input.ToString().IndexOf(' ');
	            if (space < 0) space = input.Length - 1;
                input.Insert(space, string.Format(" class=\"{0}\"", Value));
            }
        }

        class SyntaxValue: CellAttributeValue {
            public override void Format(Cell cell, StringBuilder input) {
                input.Insert(0, String.Format("<span class=\"fit_{0}\">", Value));
                input.Append("</span>");
            }
        }
    }
}
