using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseEnum: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
	        if (!type.IsEnum) return false;
	        result = new TypedValue(Enum.Parse(type, parameters.Value.Text));
            return true;
        }
    }
}
