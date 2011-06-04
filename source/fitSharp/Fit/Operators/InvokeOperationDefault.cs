// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
    public class InvokeOperationDefault: CellOperator, InvokeOperator<Cell>
    {
        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return instance.Type == typeof (CellOperationContext) && memberName == CellOperationContext.InvokeCommand;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
            var beforeCounts = new TestCounts(Processor.TestStatus.Counts);
            TypedValue result = context.DoInvoke(Processor);
            MarkCellWithLastResults(parameters, p => MarkCellWithCounts(p, beforeCounts));
            return result;
        }


        void MarkCellWithCounts(Cell target, TestCounts beforeCounts) {
            string style = Processor.TestStatus.Counts.Subtract(beforeCounts).Style;
            if (!string.IsNullOrEmpty(style) && string.IsNullOrEmpty(target.GetAttribute(CellAttribute.Status))) {
                target.SetAttribute(CellAttribute.Status, style);
            }
        }
    }
}
