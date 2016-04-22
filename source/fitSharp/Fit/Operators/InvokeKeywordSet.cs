// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeKeywordSet: CellOperator, InvokeSpecialOperator {
        public bool CanInvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return memberName.Name == "set";
        }

        public TypedValue InvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            instance.GetValue<FlowInterpreter>().ExecuteFlowRowMethod(Processor, parameters);
            return TypedValue.Void;
        }
    }
}
