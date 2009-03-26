// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text.RegularExpressions;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareEndsWith : CompareOperator<Cell> {
        private static readonly Regex matchExpression = new Regex("^\\.\\.+.*[^\\.\\.]$");

        public bool TryCompare(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters, ref bool result) {
            if (!matchExpression.IsMatch(parameters.Value.Text)) return false;
            result = instance.Value != null && instance.Value.ToString().EndsWith(GetExpected(parameters.Value.Text));
            return true;
        }

        private static string GetExpected(string text) {
            return text.Substring(2, text.Length - 2);
        }
    }
}