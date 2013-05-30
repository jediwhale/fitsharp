// Copyright © 2012 Syterra Software Inc. All rights reserved.
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
            string text = parameters.Value.Content;
            if (!today.IsStartOf(text)) return false;
            if (text.Length > today.Length) {
                string modifier = text.Substring(today.Length).Trim();
                if (!modifier.StartsWith("+") && !modifier.StartsWith("-")) return false;
            }
            return true;
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            Cell cell = parameters.Value;
            string text = cell.Content;
            int daysToAdd = 0;
            if (text.Length > today.Length) {
                string modifier = text.Substring(today.Length).Trim();
                var rest = new CellSubstring(cell, modifier.Substring(1));
                daysToAdd = Processor.Parse<Cell, int>(rest);
                if (modifier.StartsWith("-")) {
                    daysToAdd = - daysToAdd;
                }
            }
            DateTime resultDate = DateTime.Now.Date.AddDays(daysToAdd);
            return new TypedValue(resultDate);
        }
    }
}
