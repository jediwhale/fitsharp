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
                daysToAdd = (int)Parse(typeof(int), rest).Value;
                if (modifier.StartsWith("-")) {
                    daysToAdd = - daysToAdd;
                }
            }
            DateTime resultDate = DateTime.Now.Date.AddDays(daysToAdd);
            return new TypedValue(resultDate);
        }
    }
}
