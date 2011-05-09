// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
	public class ExecuteEmpty : InvokeCommandBase {
	    public override bool CanExecute(ExecuteContext context, ExecuteParameters parameters) {
	        return (context.Command == ExecuteCommand.Check)
	               && parameters.Cell.Text.Length == 0
	               && parameters.Cells.IsLeaf;
	    }

	    public override TypedValue Execute(ExecuteContext context, ExecuteParameters parameters) {
	        ShowActual(parameters, GetActual(context, parameters));
	        return TypedValue.Void;
	    }

	    static void ShowActual(ExecuteParameters parameters, object actual) {
            parameters.Cell.AddToAttribute(
                CellAttribute.InformationSuffix,
                actual == null ? "null"
	            : actual.ToString().Length == 0 ? "blank"
	            : actual.ToString());
	    }
	}
}
