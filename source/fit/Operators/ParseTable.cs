using System;
using fitlibrary.table;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseTable: ParseOperator<Cell> {
        public bool CanParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters) {
            return typeof(Table).IsAssignableFrom(type);
        }

        public TypedValue Parse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters) {
            return new TypedValue(new fitlibrary.table.ParseTable(((Parse)parameters.Value).Parts), type);
        }
    }
}
