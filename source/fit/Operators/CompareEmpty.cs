using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class CompareEmpty: CompareOperator<Cell> {
        public bool TryCompare(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters, ref bool result) {
            if ((parameters.Value.Body != null
                    && parameters.Value.Text.Length > 0)
                   || ((Parse) parameters.Value).Parts != null) return false;
            result = instance.IsNullOrEmpty;
            return true;
        }
    }
}
