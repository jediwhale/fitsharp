using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
    public class CheckOperationSymbolSave: CellOperator, InvokeOperator<Cell>
    {
        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return instance.Type == typeof (CellOperationContext) && memberName == CellOperationContext.CheckCommand
                && parameters.Value.Text.StartsWith(">>");
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
            object value = context.GetActual(Processor);
            var symbol = new Symbol(parameters.Value.Text.Substring(2), value);
            Processor.Store(symbol);

            parameters.Value.AddToAttribute(CellAttribute.InformationSuffix, value == null ? "null" : value.ToString());

            return TypedValue.Void;
        }
    }
}
