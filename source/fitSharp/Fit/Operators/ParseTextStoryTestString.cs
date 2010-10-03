// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseTextStoryTestString: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(StoryTestString);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            string content = parameters.ToString();
            if (!string.IsNullOrEmpty(parameters.Value.GetAttribute(CellAttribute.Leader))) {
                content = "<html><head><link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\"></head><body>" +
                          Environment.NewLine +
                          content + Environment.NewLine
                          + "</body></html>" + Environment.NewLine;
            }
            return new TypedValue(new StoryTestString(content));
        }
    }
}
