// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fit.Operators {
    public class ComposeHtmlStoryTestString: CellOperator, ComposeOperator<Cell> {
        public bool CanCompose(TypedValue instance) {
            return instance.Type == typeof(StoryTestString);
        }

        public Tree<Cell> Compose(TypedValue instance) {
            HtmlString.IsStandard = Processor.Get<Settings>().IsStandard;
            return Parse.ParseRootFrom(instance.ValueString);
        }
    }
}
