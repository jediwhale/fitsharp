// Copyright © 2016 Syterra Software Inc.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ComposeDefault : CellOperator, ComposeOperator<Cell> {
        public bool CanCompose(TypedValue instance) {
            return true;
        }

        public Tree<Cell> Compose(TypedValue instance) {
            var newCell = Processor.MakeCell(instance.ValueString, "td", new Tree<Cell>[] {});
            newCell.Value.SetAttribute(CellAttribute.Add, string.Empty);
            return newCell;
        }
    }
}
