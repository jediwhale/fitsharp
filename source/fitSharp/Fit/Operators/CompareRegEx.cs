// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text.RegularExpressions;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareRegEx: CellOperator, CompareOperator<Cell> {
        public bool CanCompare(TypedValue actual, Tree<Cell> expected) {
            object actualValue = actual.Value;
            if (actualValue == null) return false;
            string compareValue = expected.Value.Text;
            return compareValue.StartsWith("/") && compareValue.EndsWith("/") && actual.Type == typeof(string);
        }

        public bool Compare(TypedValue actual, Tree<Cell> expected) {
            string compareValue = expected.Value.Text;
            object actualValue = actual.Value;

            var cell = expected.Value;
            cell.AddToAttribute(CellAttribute.InformationSuffix, actualValue.ToString());

            var expectedPattern = new Regex(compareValue.Substring(1, compareValue.Length-2));
            return expectedPattern.IsMatch(actualValue.ToString());
        }
    }
}
