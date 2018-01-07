// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Fit.Operators {
    public class ComposeStoryTestHtml: CellOperator, ComposeOperator<Cell> {
        public bool CanCompose(TypedValue instance) {
            return instance.Type == typeof(StoryTestSource) && instance.GetValueAs<StoryTestSource>().Type == StoryTestType.Html;
        }

        public Tree<Cell> Compose(TypedValue instance) {
            HtmlString.IsStandard = Processor.Get<Settings>().IsStandard;
            return new HtmlTables(text => StoryTestSource.MakeTreeCell(Processor, text)).Parse(instance.ValueString);
        }
    }
}
