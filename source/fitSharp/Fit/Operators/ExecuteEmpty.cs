// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
	public class ExecuteEmpty : ExecuteBase {
	    public override bool CanExecute(ExecuteContext context, ExecuteParameters parameters) {
	        return (context.Command == ExecuteCommand.Check || context.Command == ExecuteCommand.Input)
	               && parameters.Cell.Text.Length == 0
	               && parameters.Cells.IsLeaf;
	    }

	    public override TypedValue Execute(ExecuteContext context, ExecuteParameters parameters) {
	        switch (context.Command) {
	            case ExecuteCommand.Input:
	                TypedValue actual = Processor.Invoke(context.SystemUnderTest, GetMemberName(parameters.Members),
	                                                     new TreeList<Cell>());
	                if (actual.IsValid) ShowActual(parameters, actual.Value);
	                break;

	            case ExecuteCommand.Check:
	                ShowActual(parameters, GetActual(context, parameters));
	                break;
	        }
	        return TypedValue.Void;
	    }

	    private static void ShowActual(ExecuteParameters parameters, object actual) {
            parameters.Cell.AddToAttribute(
                CellAttributes.InformationSuffixKey,
                actual == null ? "null"
	            : actual.ToString().Length == 0 ? "blank"
	            : actual.ToString(),
                CellAttributes.SuffixFormat);
	    }
	}
}