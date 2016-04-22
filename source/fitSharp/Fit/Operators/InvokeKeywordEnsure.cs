// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeKeywordEnsure: CellOperator, InvokeSpecialOperator {
        public bool CanInvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return memberName.Name == "ensure";
        }

        public TypedValue InvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            var firstCell = parameters.Branches[0].Value;
            try {
                Processor.TestStatus.ColorCell(firstCell, (bool) instance.GetValue<FlowInterpreter>().ExecuteFlowRowMethod(Processor, parameters));
            }
            catch (IgnoredException) {}
            catch (System.Exception e) {
                Processor.TestStatus.MarkException(firstCell, e);
            }
            return TypedValue.Void;
        }
    }
}
