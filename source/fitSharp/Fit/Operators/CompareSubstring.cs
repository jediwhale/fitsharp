// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text.RegularExpressions;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareSubstring : CompareOperator<Cell> {
        private static readonly Regex matchExpression = new Regex("^\\.\\..*\\.\\.$");

        public bool TryCompare(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters, ref bool result) {
            if (!matchExpression.IsMatch(parameters.Value.Text)) return false;
            result = instance.Value != null && instance.Value.ToString().IndexOf(GetExpected(parameters.Value.Text)) > -1;
            return true;
        }

        private static string GetExpected(string cell) {
            return cell.Substring(2, cell.Length - 4);
        }
    }
}