// Modified or written by Object Mentor, Inc. for inclusion with FitNesse.
// Copyright (c) 2002 Cunningham & Cunningham, Inc.
// Released under the terms of the GNU General Public License version 2 or later.

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