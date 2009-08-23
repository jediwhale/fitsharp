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

        public bool CanCompare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
            return matchExpression.IsMatch(expected.Value.Text);
        }

        public bool Compare(Processor<Cell> processor, TypedValue actual, Tree<Cell> expected) {
            return actual.Value != null && actual.Value.ToString().EndsWith(GetExpected(expected.Value.Text));
        }

        private static string GetExpected(string text) {
            return text.Substring(2, text.Length - 2);
        }
    }
}