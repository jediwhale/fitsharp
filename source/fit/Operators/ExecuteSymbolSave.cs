// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
	public class ExecuteSymbolSave : ExecuteBase {
	    public override bool TryExecute(Processor<Cell> processor, ExecuteParameters parameters, ref TypedValue result) {
			if (parameters.Verb != ExecuteParameters.Check
                || !parameters.Cell.Text.StartsWith(">>")) return false;

			object value = parameters.GetActual(processor);
            var symbol = new Symbol(parameters.Cell.Text.Substring(2), value);
            processor.Store(symbol);

	        parameters.Cell.AddToAttribute(CellAttributes.InformationSuffixKey, value == null ? "null" : value.ToString(), CellAttributes.SuffixFormat);
	        return true;
	    }
	}
}