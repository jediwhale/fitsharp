// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteSymbolSave : ExecuteBase {
        public override bool CanExecute(ExecuteParameters parameters) {
            return parameters.Verb == ExecuteParameters.Check
                && parameters.Cell.Text.StartsWith(">>");
        }

        public override TypedValue Execute(ExecuteParameters parameters) {
            object value = GetActual(parameters);
            var symbol = new Symbol(parameters.Cell.Text.Substring(2), value);
            Processor.Store(symbol);

            parameters.Cell.AddToAttribute(CellAttributes.InformationSuffixKey, value == null ? "null" : value.ToString(), CellAttributes.SuffixFormat);

            return TypedValue.Void;
        }
    }
}