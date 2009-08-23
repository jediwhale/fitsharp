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
        public bool CanCompare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
            object actualValue = actual.Value;
            if (actualValue == null) return false;
            string compareValue = expected.Value.Text;
            return compareValue.StartsWith("/") && compareValue.EndsWith("/") && actual.Type == typeof(string);
        }

        public bool Compare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
            string compareValue = expected.Value.Text;
            object actualValue = actual.Value;

            var cell = (Parse)expected.Value;
            cell.AddToAttribute(CellAttributes.InformationSuffixKey, actualValue.ToString(), CellAttributes.SuffixFormat);

            var expectedPattern = new Regex(compareValue.Substring(1, compareValue.Length-2));
            return expectedPattern.IsMatch(actualValue.ToString());
        }
    }
}
