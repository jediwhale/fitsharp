using System;
using fitlibrary.table;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseTable: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (!typeof(Table).IsAssignableFrom(type)) return false;
            result = new TypedValue(new fitlibrary.table.ParseTable(((Parse)parameters.Value).Parts), type);
            return true;
        }
    }
}
