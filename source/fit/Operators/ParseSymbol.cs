using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class ParseSymbol: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (parameters.Value == null || !parameters.Value.Text.StartsWith("<<")) return false;
            var symbol = new Symbol(parameters.Value.Text.Substring(2));
            result = new TypedValue(processor.Contains(symbol) ? processor.Load(symbol).Instance : null, type);
            //parameters.Value.SetBody((result.Value == null ? "null" : result.Value.ToString()) + Fixture.Gray("&lt;&lt;" + symbol.Id));
            parameters.Value.AddToAttribute(
                CellAttributes.InformationSuffixKey,
                result.Value == null ? "null" : result.Value.ToString(),
                CellAttributes.SuffixFormat);
            return true;
        }
    }
}