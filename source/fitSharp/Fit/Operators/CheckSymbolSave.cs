// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CheckSymbolSave: CellOperator, CheckOperator {
        public bool CanCheck(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            return expectedCell.Value.Text.StartsWith(">>");
        }

        public TypedValue Check(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            var value = actualValue.GetActual(Processor);
            Processor.Get<Symbols>().Save(expectedCell.Value.Text.Substring(2), value);

            expectedCell.Value.AddToAttribute(CellAttribute.InformationSuffix, value == null ? "null" : value.ToString());
            return TypedValue.Void;
        }
    }
}
