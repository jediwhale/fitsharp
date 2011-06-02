// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
	public class CheckOperationEmpty: CellOperator, InvokeOperator<Cell> {
	    public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
	        return instance.Type == typeof (CellOperationContext) && memberName == CellOperationContext.CheckCommand
	               && parameters.Value.Text.Length == 0
	               && parameters.IsLeaf;
	    }

	    public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
	        var actualCell = Processor.Compose(context.GetTypedActual(Processor));
	        parameters.Value.AddToAttribute(CellAttribute.InformationSuffix,
                actualCell.Value.Text.Length == 0 ? "blank" : actualCell.Value.Text); // slightly quirky behavior from original fitnesse.net
	        return TypedValue.Void;
	    }
	}
}
