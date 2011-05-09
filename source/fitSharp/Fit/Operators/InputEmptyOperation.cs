// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InputEmptyOperation: CellOperator, InvokeOperator<Cell> {
        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            if (instance.Type != typeof(InputOperation)) return false;
            var operation = instance.GetValue<InputOperation>();
            return operation.Cells.Value.Text.Length == 0 && operation.Cells.IsLeaf;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var operation = instance.GetValue<InputOperation>();
	        TypedValue actual = Processor.Invoke(operation.SystemUnderTest, GetMemberName(operation.Member),
	                                                new CellTree());
	        if (actual.IsValid) ShowActual(operation.Cells.Value, actual.Value);
	        return TypedValue.Void;
        }

        static void ShowActual(Cell cell, object actual) {
            cell.AddToAttribute(
                CellAttribute.InformationSuffix,
                actual == null ? "null"
	            : actual.ToString().Length == 0 ? "blank"
	            : actual.ToString());
	    }
    }
}
