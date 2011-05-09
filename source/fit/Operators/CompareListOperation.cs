// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators
{
    public class CompareListOperation: CellOperator, InvokeOperator<Cell>
    {
        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            if (instance.Type != typeof(CompareOperation)) return false;
            var operation = instance.GetValue<CompareOperation>();
            var cell = (Parse)operation.Cells.Value;
            return cell.Parts != null && typeof(IList).IsAssignableFrom(operation.Target.Type);
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var operation = instance.GetValue<CompareOperation>();
            var cell = (Parse)operation.Cells.Value;
            var matcher = new ListMatcher(Processor, new ArrayMatchStrategy(Processor, cell.Parts.Parts));
            return new TypedValue(matcher.IsEqual(operation.Target.Value, cell));
        }
    }
}
