// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
	public class ExecuteEmpty : ExecuteBase {
	    public override bool CanExecute(ExecuteParameters parameters) {
	        return (parameters.Verb == ExecuteParameters.Check || parameters.Verb == ExecuteParameters.Input)
	               && parameters.Cell.Text.Length == 0
	               && parameters.Cells.IsLeaf;
	    }

	    public override TypedValue Execute(ExecuteParameters parameters) {
	        switch (parameters.Verb) {
	            case ExecuteParameters.Input:
                    TypedValue actual = Processor.Invoke(parameters.SystemUnderTest, GetMemberName(parameters.Members),
                             new TreeList<Cell>());
                    if (actual.IsValid) ShowActual(parameters, actual.Value);
	                break;
                case ExecuteParameters.Check:
			        ShowActual(parameters, GetActual(parameters));
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