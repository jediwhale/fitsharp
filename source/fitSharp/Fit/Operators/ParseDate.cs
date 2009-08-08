using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseDate: ParseOperator<Cell> {
        private static readonly IdentifierName today = new IdentifierName("today");

        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (type != typeof(DateTime)) return false;
            Cell cell = parameters.Value;
            if (!today.IsStartOf(cell.Text)) return false;
            int daysToAdd = 0;
            if (cell.Text.Length > today.Length) {
                string modifier = cell.Text.Substring(today.Length).Trim();
                if (modifier.StartsWith("+") || modifier.StartsWith("-")) {
                    var rest = new CellSubstring(cell, modifier.Substring(1));
                    daysToAdd = (int)processor.Parse(typeof(int), rest).Value;
                    if (modifier.StartsWith("-")) {
                        daysToAdd = - daysToAdd;
                    }
                }
                else return false;
            }
            DateTime resultDate = DateTime.Now.Date.AddDays(daysToAdd);
            result = new TypedValue(resultDate);
            return true;
        }
    }
}
