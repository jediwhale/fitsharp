// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
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

        void MarkCellWithLastResults(Tree<Cell> parameters, Action<Cell> markWithCounts) {
            var cell = parameters.IsLeaf ? null : parameters.Branches[0].Value; //todo: maybe
            if (cell != null && !string.IsNullOrEmpty(Processor.TestStatus.LastAction)) {
                cell.SetAttribute(CellAttribute.Folded, Processor.TestStatus.LastAction);
                markWithCounts(cell);
            }
            Processor.TestStatus.LastAction = null;
        }

        void MarkCellWithCounts(Cell target, TestCounts beforeCounts) {
            string style = Processor.TestStatus.Counts.Subtract(beforeCounts).Style;
            if (!string.IsNullOrEmpty(style) && string.IsNullOrEmpty(target.GetAttribute(CellAttribute.Status))) {
                target.SetAttribute(CellAttribute.Status, style);
            }
        }
    }
}
