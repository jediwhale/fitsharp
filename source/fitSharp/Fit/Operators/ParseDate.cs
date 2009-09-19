// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseDate: CellOperator, ParseOperator<Cell> {
        private static readonly IdentifierName today = new IdentifierName("today");

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            if (type != typeof(DateTime)) return false;
            Cell cell = parameters.Value;
            if (!today.IsStartOf(cell.Text)) return false;
            if (cell.Text.Length > today.Length) {
                string modifier = cell.Text.Substring(today.Length).Trim();
                if (!modifier.StartsWith("+") && !modifier.StartsWith("-")) return false;
            }
            return true;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            Cell cell = parameters.Value;
            int daysToAdd = 0;
            if (cell.Text.Length > today.Length) {
                string modifier = cell.Text.Substring(today.Length).Trim();
                var rest = new CellSubstring(cell, modifier.Substring(1));
                daysToAdd = Parse<int>(rest);
                if (modifier.StartsWith("-")) {
                    daysToAdd = - daysToAdd;
                }
            }
            DateTime resultDate = DateTime.Now.Date.AddDays(daysToAdd);
            return new TypedValue(resultDate);
        }
    }
}
