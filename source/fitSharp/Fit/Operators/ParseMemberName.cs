using System;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseMemberName: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (type != typeof(MemberName)) return false;
            var nameParts = new StringBuilder();
            foreach (Cell namePart in parameters.Leaves) {
                nameParts.Append(namePart.Text);
            }
            result = new TypedValue(new MemberName(new GracefulName(nameParts.ToString()).IdentifierName.ToString()));

            return true;
        }
    }
}