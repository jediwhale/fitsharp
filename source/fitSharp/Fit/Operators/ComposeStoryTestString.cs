// Copyright © 2016 Syterra Software Inc. All rights reserved.
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
    public class ComposeStoryTestString: CellOperator, ComposeOperator<Cell> {
        public bool CanCompose(TypedValue instance) {
            return instance.Type == typeof(StoryTestString);
        }

        public Tree<Cell> Compose(TypedValue instance) {
            var storyTestInput = instance.ValueString;
            if (storyTestInput.Contains(Characters.TextStoryTestBegin)) {
                return new TextTables(
                        new TextTableScanner(instance.ValueString, c => c == CharacterType.Letter),
                        MakeTreeCell)
                    .Parse();
            }
            else {
                HtmlString.IsStandard = Processor.Get<Settings>().IsStandard;
                return new HtmlTables(MakeTreeCell).Parse(instance.ValueString);
            }
        }

        Tree<Cell> MakeTreeCell(string text) {
            return Processor.MakeCell(text, string.Empty, new TreeList<Cell>[] {});
        } 
    }
}
