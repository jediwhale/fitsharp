// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text.RegularExpressions;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class CompareRegEx: CompareOperator<Cell> {
        public bool TryCompare(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters, ref bool result) {
            string compareValue = parameters.Value.Text;
            if (!compareValue.StartsWith("/") || !compareValue.EndsWith("/") || instance.Type != typeof(string)) return false;

            object actualValue = instance.Value;
            if (actualValue == null) return false;

            var cell = (Parse)parameters.Value;
            cell.AddToAttribute(CellAttributes.InformationSuffixKey, actualValue.ToString(), CellAttributes.SuffixFormat);

            var expected = new Regex(compareValue.Substring(1, compareValue.Length-2));
            result = expected.IsMatch(actualValue.ToString());
            return true;
        }
    }
}
