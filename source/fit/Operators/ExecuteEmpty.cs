// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;

namespace fit.Operators {
	public class ExecuteEmpty : ExecuteBase {
	    public override bool CanExecute(ExecuteParameters parameters) {
			return (parameters.Verb == ExecuteParameters.Check || parameters.Verb == ExecuteParameters.Input)
                && parameters.Cell.Text.Length == 0
                && ((Parse)parameters.Cell).Parts == null;
	    }

	    public override TypedValue Execute(ExecuteParameters parameters) {
	        switch (parameters.Verb) {
	            case ExecuteParameters.Input:
                    TypedValue actual = Processor.TryInvoke(parameters.SystemUnderTest, parameters.GetMemberName(Processor),
                             new TreeList<Cell>());
                    if (actual.IsValid) ShowActual(parameters, actual.Value);
	                break;
                case ExecuteParameters.Check:
			        ShowActual(parameters, parameters.GetActual(Processor));
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