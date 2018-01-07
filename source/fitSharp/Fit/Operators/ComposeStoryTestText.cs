// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Fit.Operators {
    public class ComposeStoryTestText: CellOperator, ComposeOperator<Cell> {
        public bool CanCompose(TypedValue instance) {
            return instance.Type == typeof(StoryTestSource) && instance.GetValueAs<StoryTestSource>().Type == StoryTestType.Text;
        }

        public Tree<Cell> Compose(TypedValue instance) {
            return new TextTables(
                    new TextTableScanner(instance.ValueString, c => c == CharacterType.Letter),
                    text => StoryTestSource.MakeTreeCell(Processor, text))
                .Parse();
        }
    }
}
