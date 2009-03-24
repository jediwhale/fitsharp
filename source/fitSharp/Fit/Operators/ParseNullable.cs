using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseNullable: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (!type.IsGenericType || !type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) return false;
            result = parameters.Value.Text.Length > 0
                ? processor.Parse(type.GetGenericArguments()[0], parameters)
                : new TypedValue(null, type);
            return true;
        }
    }
}
