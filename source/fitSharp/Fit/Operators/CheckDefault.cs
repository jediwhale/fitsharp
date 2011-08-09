// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CheckDefault: CellOperator, CheckOperator {
        public bool CanCheck(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            return true;
        }

        public TypedValue Check(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            try {
                var actual = actualValue.GetTypedActual(Processor);
                if (Processor.Compare(actual, expectedCell)) {
                    Processor.TestStatus.MarkRight(expectedCell.Value);
                }
                else {
                    var actualCell = Processor.Compose(actual);
                    Processor.TestStatus.MarkWrong(expectedCell.Value, actualCell.Value.Text);
                }
            }
            catch (IgnoredException) {}
            MarkCellWithLastResults(expectedCell, p => {});
            return TypedValue.Void;
        }
    }
}
