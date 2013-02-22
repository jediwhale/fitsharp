// Copyright © 2013 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
	public class CheckEmpty: CellOperator, CheckOperator {
        public bool CanCheck(CellOperationValue actualValue, Tree<Cell> expectedCell) {
	        return expectedCell.Value.Text.Length == 0
	               && expectedCell.IsLeaf;
	    }

        public TypedValue Check(CellOperationValue actualValue, Tree<Cell> expectedCell) {
	        var actualCell = Processor.Compose(actualValue.GetTypedActual(Processor));
	        expectedCell.Value.SetAttribute(CellAttribute.InformationSuffix,
                actualCell.Value.Text.Length == 0 ? "blank" : actualCell.Value.Text); // slightly quirky behavior from original fitnesse.net
            return new TypedValue(true);
        }
	}
}
